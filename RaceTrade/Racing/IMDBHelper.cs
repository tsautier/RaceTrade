using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaceTrade;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Globalization;

namespace RaceTrader
{
    /// <summary>
    /// Helper class for IMDB API integration using imdbapi.dev (FREE)
    /// Provides movie lookup and information retrieval
    /// API Documentation: https://imdbapi.dev/
    /// </summary>
    public static class IMDBHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string IMDB_API_BASE = "https://api.imdbapi.dev";

        private static DateTime lastRequestTime = DateTime.MinValue;
        private static readonly TimeSpan minRequestInterval = TimeSpan.FromMilliseconds(500);

        static IMDBHelper()
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RaceTrader/1.0");
            httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        #region Rate Limiting

        private static readonly object rateLimitLock = new object();

        private static async Task RateLimitDelay()
        {
            // Releases are processed concurrently; compute and reserve the next slot
            // atomically, otherwise the interval check is defeated and we burst into
            // the API's rate limiter (HTTP 429).
            TimeSpan delay;
            lock (rateLimitLock)
            {
                var now = DateTime.Now;
                var earliestNext = lastRequestTime + minRequestInterval;
                delay = earliestNext > now ? earliestNext - now : TimeSpan.Zero;

                // Reserve our slot up front so concurrent callers stagger.
                lastRequestTime = now + delay;
            }

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay);
        }

        #endregion

        #region API Calls

        /// <summary>
        /// Lookup movie by IMDB ID (with caching)
        /// </summary>
        public static async Task<IMDBMovie> LookupByImdb(string imdbId, int cacheDays = 30)
        {
            if (string.IsNullOrEmpty(imdbId))
                return null;

            // Ensure IMDB ID has correct format
            if (!imdbId.StartsWith("tt", StringComparison.OrdinalIgnoreCase))
            {
                imdbId = "tt" + imdbId;
            }

            // Try cache first
            var cached = IMDBCache.GetCachedMovieByImdb(imdbId, cacheDays);
            if (cached != null)
            {
                return cached;
            }

            try
            {
                await RateLimitDelay();

                // imdbapi.dev endpoint: GET /titles/{titleId}
                var url = $"{IMDB_API_BASE}/titles/{imdbId}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);

                    // Check if movie was found
                    if (result["id"] != null)
                    {
                        var movie = ParseImdbApiResponse(result);

                        // Cache the result
                        if (movie != null)
                        {
                            IMDBCache.CacheMovie(movie);
                        }

                        return movie;
                    }

                    LogManager.Warning($"Movie not found on IMDB: {imdbId}");
                    IMDBCache.CacheNotFound(imdbId);
                    return null;
                }

                LogManager.Error($"IMDB API error: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error looking up IMDB {imdbId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Search for movie by title (with caching)
        /// </summary>
        public static async Task<IMDBMovie> SearchMovie(string title, int? year = null, int cacheDays = 30)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            // Check if recently not found
            var searchKey = year.HasValue ? $"{title}|{year}" : title;
            if (IMDBCache.IsRecentlyNotFound(searchKey, 7))
            {
                return null;
            }

            // Try cache first
            var cached = IMDBCache.GetCachedMovieByTitle(title, year, cacheDays);
            if (cached != null)
            {
                return cached;
            }

            try
            {
                await RateLimitDelay();

                // imdbapi.dev search endpoint: GET /search/titles?query={query}&limit={limit}
                var url = $"{IMDB_API_BASE}/search/titles?query={Uri.EscapeDataString(title)}&limit=10";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);

                    // Get search results array
                    var results = result["titles"] as JArray;
                    if (results != null && results.Any())
                    {
                        // Find best match (prefer exact year match if provided)
                        JToken bestMatch = null;

                        if (year.HasValue)
                        {
                            bestMatch = results.FirstOrDefault(r =>
                            {
                                var resultYear = r["startYear"]?.ToObject<int?>();
                                return resultYear == year;
                            });
                        }

                        // If no year match or no year specified, take first movie result
                        if (bestMatch == null)
                        {
                            bestMatch = results.FirstOrDefault(r =>
                                r["type"]?.ToString().Equals("movie", StringComparison.OrdinalIgnoreCase) == true);
                        }

                        // Fall back to first result if no movie found
                        if (bestMatch == null)
                        {
                            bestMatch = results[0];
                        }

                        // Search hits are SPARSE (no languages/countries/directors) -
                        // caching them as full records made only_english/only_us_country
                        // filters block everything for 30 days. Fetch the full record.
                        var matchedId = (bestMatch as JObject)?["id"]?.ToString();
                        if (!string.IsNullOrEmpty(matchedId))
                        {
                            var detailed = await LookupByImdb(matchedId, cacheDays);
                            if (detailed != null)
                            {
                                IMDBCache.CacheSearch(title, year, detailed.ImdbID);
                                return detailed;
                            }
                        }

                        // Fallback: parse the sparse search result directly
                        var movie = ParseImdbApiResponse(bestMatch as JObject);

                        if (movie != null)
                        {
                            IMDBCache.CacheMovie(movie);
                            IMDBCache.CacheSearch(title, year, movie.ImdbID);
                            return movie;
                        }
                    }
                    else
                    {
                        // Cache not found
                        IMDBCache.CacheNotFound(searchKey);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error searching for movie '{title}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parse imdbapi.dev response into IMDBMovie object
        /// </summary>
        private static IMDBMovie ParseImdbApiResponse(JObject json)
        {
            if (json == null)
                return null;

            var movie = new IMDBMovie
            {
                ImdbID = json["id"]?.ToString(),
                Title = json["primaryTitle"]?.ToString(),
                Year = json["startYear"]?.ToString(),
                Runtime = ConvertRuntimeToString(json["runtimeSeconds"]?.ToObject<int?>()),
                Plot = json["plot"]?.ToString(),
                Poster = json["primaryImage"]?["url"]?.ToString(),
                Type = json["type"]?.ToString() ?? "movie"
            };

            // Parse rating
            // rounds to 1 decimal during parsing
            var ratingObj = json["rating"] as JObject;
            if (ratingObj != null)
            {
                var aggregateRating = ratingObj["aggregateRating"]?.ToObject<float?>();
                if (aggregateRating.HasValue)
                {
                    // Round to 1 decimal place to avoid float->double precision issues
                    movie.ImdbRating = Math.Round((double)aggregateRating.Value, 1);
                }

                var voteCount = ratingObj["voteCount"]?.ToObject<int?>();
                if (voteCount.HasValue)
                {
                    movie.ImdbVotes = voteCount.Value;
                }
            }

            // Parse genres array into comma-separated string
            var genresArray = json["genres"] as JArray;
            if (genresArray != null && genresArray.Any())
            {
                movie.Genres = genresArray.Select(g => g.ToString()).ToList();
                movie.Genre = string.Join(", ", movie.Genres);
            }

            // Parse directors
            var directorsArray = json["directors"] as JArray;
            if (directorsArray != null && directorsArray.Any())
            {
                var directorNames = directorsArray
                    .Select(d => d["displayName"]?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                movie.Director = string.Join(", ", directorNames);
            }

            // Parse stars/actors
            var starsArray = json["stars"] as JArray;
            if (starsArray != null && starsArray.Any())
            {
                var starNames = starsArray
                    .Select(s => s["displayName"]?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                movie.Actors = string.Join(", ", starNames);
            }

            // Parse origin countries
            var countriesArray = json["originCountries"] as JArray;
            if (countriesArray != null && countriesArray.Any())
            {
                movie.Countries = countriesArray
                    .Select(c => c["name"]?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                movie.Country = string.Join(", ", movie.Countries);
            }

            // Parse spoken languages
            var languagesArray = json["spokenLanguages"] as JArray;
            if (languagesArray != null && languagesArray.Any())
            {
                movie.Languages = languagesArray
                    .Select(l => l["name"]?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                movie.Language = string.Join(", ", movie.Languages);
            }

            return movie;
        }

        /// <summary>
        /// Convert runtime in seconds to human-readable string (e.g., "142 min")
        /// </summary>
        private static string ConvertRuntimeToString(int? runtimeSeconds)
        {
            if (!runtimeSeconds.HasValue || runtimeSeconds.Value == 0)
                return null;

            int minutes = runtimeSeconds.Value / 60;
            return $"{minutes} min";
        }

        #endregion

        #region Additional Release Information Extraction

        /// <summary>
        /// Extract codec from release name (x264, x265, H264, H265, XviD, VP8, VP9)
        /// </summary>
        public static string ExtractCodec(string releaseName)
        {
            var match = Regex.Match(releaseName, @"[\s._-]([xh]26[45]|xvid|VP[89])[\s._-]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }
            return null;
        }

        /// <summary>
        /// Extract source from release name (BluRay, WEB-DL, HDTV, etc.)
        /// </summary>
        public static string ExtractSource(string releaseName)
        {
            // Check for complete BluRay first
            if (Regex.IsMatch(releaseName, @"[\s._-](((720p|1080p)\.(PURE\.)?M?BLURAY)|COMPLETE(\.PURE)?\.M?BLURAY)", RegexOptions.IgnoreCase))
            {
                return "BLURAY";
            }

            if (Regex.IsMatch(releaseName, @"[\s._-]((2160p\.UHD\.M?BLURAY)|COMPLETE(\.UHD)?\.M?BLURAY)", RegexOptions.IgnoreCase))
            {
                return "UHD.BLURAY";
            }

            // Check SD sources
            var match = Regex.Match(releaseName, @"[\s._-](DVDRIP|BDRIP)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }

            // Check other sources
            match = Regex.Match(releaseName, @"[\s._-]([AU]?HDTV|AUHDTV|PDTV|DSR|WEBRIP|WEB)[\s._-]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }

            return null;
        }

        /// <summary>
        /// Extract resolution from release name (720P, 1080P, 2160P, etc.)
        /// </summary>
        public static string ExtractResolution(string releaseName)
        {
            var match = Regex.Match(releaseName, @"[\s._-](720P|1080P|1280P|1440P|1920P|2160P|2300P|2700P|2880P)[\s._-]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }
            return null;
        }

        /// <summary>
        /// Extract HDR range from release name (HDR, DV, HLG, DV.HDR)
        /// </summary>
        public static string ExtractRange(string releaseName)
        {
            var match = Regex.Match(releaseName, @"[\s._-](DV\.HDR|HDR|DV|HLG)[\s._-]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToUpper();
            }
            return null;
        }

        /// <summary>
        /// Check if release is INTERNAL
        /// </summary>
        public static bool IsInternal(string releaseName)
        {
            // Check for INTERNAL or INT tag
            if (Regex.IsMatch(releaseName, @"[\s._-](INTERNAL|INT)[\s._-]", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Check if group ends with _INT
            var group = ExtractGroup(releaseName);
            if (!string.IsNullOrEmpty(group) && group.ToUpper().EndsWith("_INT"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if release is MULTI language
        /// </summary>
        public static bool IsMulti(string releaseName)
        {
            if (string.IsNullOrEmpty(releaseName))
                return false;

            // Must be a delimited MULTi tag, not any title containing the substring
            // (e.g. "Multitude") - same approach as IsInternal.
            return Regex.IsMatch(releaseName, @"[\s._-]MULTI([\s._-]|$)", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Extract group from release name (the part after the last dash)
        /// </summary>
        public static string ExtractGroup(string releaseName)
        {
            var match = Regex.Match(releaseName, @"-([^-]+)$");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        /// <summary>
        /// Extract repeat/dupe tags (PROPER, REPACK, RERIP, REAL.PROPER)
        /// </summary>
        public static string ExtractRepeatTag(string releaseName)
        {
            // Remove group first
            var nameWithoutGroup = Regex.Replace(releaseName, @"-[^-]+$", "");

            var tags = new[] { "REAL.PROPER", "PROPER", "RERIP", "REPACK" };
            foreach (var tag in tags)
            {
                if (nameWithoutGroup.ToUpper().Contains(tag))
                {
                    return tag;
                }
            }
            return null;
        }

        #endregion

        #region Release Name Parsing

        /// <summary>
        /// Extract movie title from release name
        /// Extracts everything before the year
        /// </summary>
        public static string ExtractTitleFromRelease(string releaseName)
        {
            if (string.IsNullOrEmpty(releaseName))
                return null;

            var name = System.IO.Path.GetFileNameWithoutExtension(releaseName);

            // Extract everything before the year
            var match = Regex.Match(name, @"^(.+?)[\s._-](19\d{2}|20\d{2})[\s._-]");
            if (match.Success)
            {
                name = match.Groups[1].Value;
            }
            // If no year found, just use the whole name
            // The release name itself will be the title

            // Replace dots/underscores with spaces
            name = name.Replace('.', ' ').Replace('_', ' ');

            return name.Trim();
        }

        /// <summary>
        /// Extract year from release name
        /// </summary>
        public static int? ExtractYearFromRelease(string releaseName)
        {
            if (string.IsNullOrEmpty(releaseName))
                return null;

            // Look for 4-digit year (1900-2099)
            var match = Regex.Match(releaseName, @"\b(19\d{2}|20\d{2})\b");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int year))
            {
                return year;
            }

            return null;
        }

        #endregion

        #region Release Validation

        /// <summary>
        /// Enriches release information with IMDB data
        /// </summary>
        public static async Task<MovieReleaseInfo> EnrichReleaseInfo(string releaseName)
        {
            var releaseInfo = new MovieReleaseInfo
            {
                ReleaseName = releaseName,
                IsValid = false
            };

            // Extract title and year, then search
            var title = ExtractTitleFromRelease(releaseName);
            var year = ExtractYearFromRelease(releaseName);

            if (!string.IsNullOrEmpty(title))
            {
                releaseInfo.Title = title;
                releaseInfo.Year = year;
                releaseInfo.Movie = await SearchMovie(title, year);
            }

            // Extract release metadata
            releaseInfo.Codec = ExtractCodec(releaseName);
            releaseInfo.Source = ExtractSource(releaseName);
            releaseInfo.Resolution = ExtractResolution(releaseName);
            releaseInfo.Range = ExtractRange(releaseName);
            releaseInfo.Group = ExtractGroup(releaseName);
            releaseInfo.RepeatTag = ExtractRepeatTag(releaseName);
            releaseInfo.IsInternal = IsInternal(releaseName);
            releaseInfo.IsMulti = IsMulti(releaseName);

            releaseInfo.IsValid = releaseInfo.Movie != null;

            return releaseInfo;
        }

        /// <summary>
        /// Check if release should be raced based on IMDB data and release metadata
        /// </summary>
        public static bool ShouldRace(MovieReleaseInfo releaseInfo, IMDBConfig config)
        {
            if (config == null)
                config = new IMDBConfig { FallbackOnError = true };

            var movie = releaseInfo?.Movie;

            // Check movie data
            if (movie != null)
            {
                // Check rating
                if (config.MinRating > 0 && movie.ImdbRating.HasValue)
                {
                    if (movie.ImdbRating.Value < config.MinRating)
                    {
                        LogManager.Warning($"  ❌ Rating {movie.ImdbRating.Value} below minimum {config.MinRating}");
                        return false;
                    }
                }

                // Check votes
                if (config.MinVotes > 0 && movie.ImdbVotes.HasValue)
                {
                    if (movie.ImdbVotes.Value < config.MinVotes)
                    {
                        LogManager.Warning($"  ❌ Votes {movie.ImdbVotes.Value} below minimum {config.MinVotes}");
                        return false;
                    }
                }

                // Check genres - allowed
                if (config.AllowedGenres != null && config.AllowedGenres.Any())
                {
                    if (movie.Genres == null || !movie.Genres.Any())
                    {
                        LogManager.Warning("  ❌ No genre information available");
                        return config.FallbackOnError;
                    }

                    var hasAllowedGenre = movie.Genres.Any(g =>
                        config.AllowedGenres.Contains(g, StringComparer.OrdinalIgnoreCase));

                    if (!hasAllowedGenre)
                    {
                        LogManager.Warning($"  ❌ Genre not in allowed list: {string.Join(", ", movie.Genres)}");
                        return false;
                    }
                }

                // Check genres - blocked
                if (config.BlockedGenres != null && config.BlockedGenres.Any() && movie.Genres != null)
                {
                    var hasBlockedGenre = movie.Genres.Any(g =>
                        config.BlockedGenres.Contains(g, StringComparer.OrdinalIgnoreCase));

                    if (hasBlockedGenre)
                    {
                        LogManager.Warning($"  ❌ Genre is blocked: {string.Join(", ", movie.Genres)}");
                        return false;
                    }
                }

                // Check language - only English
                if (config.OnlyEnglish && movie.Languages != null)
                {
                    if (!movie.Languages.Any(l => l.Equals("English", StringComparison.OrdinalIgnoreCase)))
                    {
                        LogManager.Warning($"  ❌ Not English language: {string.Join(", ", movie.Languages)}");
                        return false;
                    }
                }

                // Check country - only US
                if (config.OnlyUSCountry && movie.Countries != null)
                {
                    if (!movie.Countries.Any(c => c.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                                                  c.Equals("USA", StringComparison.OrdinalIgnoreCase)))
                    {
                        LogManager.Warning($"  ❌ Not US production: {string.Join(", ", movie.Countries)}");
                        return false;
                    }
                }
            }
            else if (!config.FallbackOnError)
            {
                LogManager.Warning("  ❌ No movie data found and FallbackOnError is false");
                return false;
            }

            // Check release metadata filters

            // Resolution
            if (config.AllowedResolutions != null && config.AllowedResolutions.Any())
            {
                if (string.IsNullOrEmpty(releaseInfo.Resolution) ||
                    !config.AllowedResolutions.Contains(releaseInfo.Resolution, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Resolution '{releaseInfo.Resolution}' not in allowed list");
                    return false;
                }
            }
            if (config.BlockedResolutions != null && config.BlockedResolutions.Any())
            {
                if (!string.IsNullOrEmpty(releaseInfo.Resolution) &&
                    config.BlockedResolutions.Contains(releaseInfo.Resolution, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Resolution '{releaseInfo.Resolution}' is blocked");
                    return false;
                }
            }

            // Source
            if (config.AllowedSources != null && config.AllowedSources.Any())
            {
                if (string.IsNullOrEmpty(releaseInfo.Source) ||
                    !config.AllowedSources.Contains(releaseInfo.Source, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Source '{releaseInfo.Source}' not in allowed list");
                    return false;
                }
            }
            if (config.BlockedSources != null && config.BlockedSources.Any())
            {
                if (!string.IsNullOrEmpty(releaseInfo.Source) &&
                    config.BlockedSources.Contains(releaseInfo.Source, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Source '{releaseInfo.Source}' is blocked");
                    return false;
                }
            }

            // Codec
            if (config.AllowedCodecs != null && config.AllowedCodecs.Any())
            {
                if (string.IsNullOrEmpty(releaseInfo.Codec) ||
                    !config.AllowedCodecs.Contains(releaseInfo.Codec, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Codec '{releaseInfo.Codec}' not in allowed list");
                    return false;
                }
            }
            if (config.BlockedCodecs != null && config.BlockedCodecs.Any())
            {
                if (!string.IsNullOrEmpty(releaseInfo.Codec) &&
                    config.BlockedCodecs.Contains(releaseInfo.Codec, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Codec '{releaseInfo.Codec}' is blocked");
                    return false;
                }
            }

            // Group
            if (config.AllowedGroups != null && config.AllowedGroups.Any())
            {
                if (string.IsNullOrEmpty(releaseInfo.Group) ||
                    !config.AllowedGroups.Contains(releaseInfo.Group, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Group '{releaseInfo.Group}' not in allowed list");
                    return false;
                }
            }
            if (config.BlockedGroups != null && config.BlockedGroups.Any())
            {
                if (!string.IsNullOrEmpty(releaseInfo.Group) &&
                    config.BlockedGroups.Contains(releaseInfo.Group, StringComparer.OrdinalIgnoreCase))
                {
                    LogManager.Warning($"  ❌ Group '{releaseInfo.Group}' is blocked");
                    return false;
                }
            }

            // Internal
            if (config.SkipInternals && releaseInfo.IsInternal)
            {
                LogManager.Warning("  ❌ Internal release (skipping internals)");
                return false;
            }
            if (config.OnlyInternals && !releaseInfo.IsInternal)
            {
                LogManager.Warning("  ❌ Not internal (only racing internals)");
                return false;
            }

            // HDR
            if (config.SkipHDR && !string.IsNullOrEmpty(releaseInfo.Range))
            {
                LogManager.Warning($"  ❌ HDR/DV release '{releaseInfo.Range}' (skipping HDR)");
                return false;
            }
            if (config.OnlyHDR && string.IsNullOrEmpty(releaseInfo.Range))
            {
                LogManager.Warning("  ❌ Not HDR (only racing HDR)");
                return false;
            }

            // Propers/Repacks
            if (config.SkipPropers && !string.IsNullOrEmpty(releaseInfo.RepeatTag))
            {
                LogManager.Warning($"  ❌ {releaseInfo.RepeatTag} release (skipping propers)");
                return false;
            }

            return true;
        }

        #endregion
    }

    #region Data Models

    public class IMDBMovie
    {
        public string ImdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string Metascore { get; set; }
        public string Type { get; set; }
        public double? ImdbRating { get; set; }
        public int? ImdbVotes { get; set; }

        // Parsed lists
        public List<string> Genres { get; set; }
        public List<string> Languages { get; set; }
        public List<string> Countries { get; set; }
    }

    public class MovieReleaseInfo
    {
        public string ReleaseName { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public IMDBMovie Movie { get; set; }
        public bool IsValid { get; set; }

        // Release metadata
        public string Codec { get; set; }
        public string Source { get; set; }
        public string Resolution { get; set; }
        public string Range { get; set; }
        public string Group { get; set; }
        public string RepeatTag { get; set; }
        public bool IsInternal { get; set; }
        public bool IsMulti { get; set; }
    }

    public class IMDBConfig
    {
        public bool Enabled { get; set; }
        public double MinRating { get; set; }
        public int MinVotes { get; set; }
        public List<string> AllowedGenres { get; set; }
        public List<string> BlockedGenres { get; set; }
        public bool OnlyEnglish { get; set; }
        public bool OnlyUSCountry { get; set; }
        public bool NoDocumentary { get; set; }
        public bool NoMusic { get; set; }
        public bool NoComedy { get; set; }
        public bool NoShow { get; set; }
        public bool FallbackOnError { get; set; }

        // Release metadata filters
        public List<string> AllowedResolutions { get; set; }  // e.g., ["1080P", "2160P"]
        public List<string> BlockedResolutions { get; set; }
        public List<string> AllowedSources { get; set; }      // e.g., ["WEB", "BLURAY"]
        public List<string> BlockedSources { get; set; }
        public List<string> AllowedCodecs { get; set; }       // e.g., ["X265", "H265"]
        public List<string> BlockedCodecs { get; set; }
        public List<string> AllowedGroups { get; set; }       // e.g., ["KILLERS", "ROVERS"]
        public List<string> BlockedGroups { get; set; }
        public bool SkipInternals { get; set; }
        public bool OnlyInternals { get; set; }
        public bool SkipHDR { get; set; }
        public bool OnlyHDR { get; set; }
        public bool SkipPropers { get; set; }                 // Skip PROPER/REPACK/RERIP
    }

    #endregion
}