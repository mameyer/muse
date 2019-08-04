using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentSpotifyApi;
using FluentSpotifyApi.Builder.Browse;
using Microsoft.AspNetCore.Mvc;
using Muse.Helpers;
using Muse.Models;

namespace Muse.Controllers.API
{
    public class RecommendationApiController : BaseApiController
    {
        public RecommendationApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        public async Task<Recommendation> Get([ModelBinder(typeof(NestedModelBinder<RecommendationOptions>))]RecommendationOptions options)
        {
            const int runs = 1;
            const int max = 20;

            Recommendation recommendation = new Recommendation();
            recommendation.Songs = new List<Song>();
            recommendation.SeedArtists = options.Artists;
            recommendation.SeedGenres = options.Genres;
            recommendation.SeedTracks = options.Tracks;

            if (options == null)
            {
                return recommendation;
            }

            // only 5 parameter for seeding available..
            const int maxSeedParams = 5;
            int sumParams = recommendation.SeedArtists.Count() + recommendation.SeedGenres.Count() + recommendation.SeedTracks.Count();
            if (sumParams > maxSeedParams)
            {
                if (recommendation.SeedGenres.Count() > maxSeedParams)
                {
                    recommendation.SeedGenres = recommendation.SeedGenres.Take(5).ToList();
                    recommendation.SeedArtists.Clear();
                    recommendation.SeedTracks.Clear();
                }
                else
                {
                    int left = maxSeedParams - recommendation.SeedGenres.Count();
                    if (recommendation.SeedArtists.Count() > left)
                    {
                        recommendation.SeedArtists = recommendation.SeedArtists.Take(left).ToList();
                        recommendation.SeedTracks.Clear();
                    }
                    else
                    {
                        left -= recommendation.SeedArtists.Count();
                        if (recommendation.SeedTracks.Count() > left)
                        {
                            recommendation.SeedTracks = recommendation.SeedTracks.Take(left).ToList();
                        }
                    }
                }
            }

            for (int i=0; i<runs; i+=1)
            {
                Action<ITuneableTrackAttributesBuilder> buildTunableTrackAttributes = new Action<ITuneableTrackAttributesBuilder>(builder =>
                {
                    // A confidence measure from 0.0 to 1.0 of whether the track is acoustic. 1.0 represents high confidence the track is acoustic.
                    builder.Acousticness(a => { a.Min(options.Acousticness.Min); a.Max(options.Acousticness.Max); a.Target(options.Acousticness.Target); });
                    // Danceability describes how suitable a track is for dancing based on a combination of musical elements including tempo, rhythm stability, beat strength, and overall regularity. A value of 0.0 is least danceable and 1.0 is most danceable.
                    builder.Danceability(d => { d.Min(options.Danceability.Min); d.Max(options.Danceability.Max); d.Target(options.Danceability.Target); });
                    // The duration of the track in milliseconds.
                    builder.DurationMs(d => { d.Min(options.DurationMs.Min); d.Max(options.DurationMs.Max); d.Target(options.DurationMs.Target); });
                    // Energy is a measure from 0.0 to 1.0 and represents a perceptual measure of intensity and activity. Typically, energetic tracks feel fast, loud, and noisy. For example, death metal has high energy, while a Bach prelude scores low on the scale. Perceptual features contributing to this attribute include dynamic range, perceived loudness, timbre, onset rate, and general entropy.
                    builder.Energy(e => { e.Min(options.Energy.Min); e.Max(options.Energy.Max); e.Target(options.Energy.Target); });
                    //Predicts whether a track contains no vocals. “Ooh” and “aah” sounds are treated as instrumental in this context. Rap or spoken word tracks are clearly “vocal”. The closer the instrumentalness value is to 1.0, the greater likelihood the track contains no vocal content. Values above 0.5 are intended to represent instrumental tracks, but confidence is higher as the value approaches 1.0.
                    builder.Instrumentalness(inst => { inst.Min(options.Instrumentalness.Min); inst.Max(options.Instrumentalness.Max); inst.Target(options.Instrumentalness.Target); });
                    // The key the track is in. Integers map to pitches using standard Pitch Class notation. E.g. 0 = C, 1 = C♯/D♭, 2 = D, and so on.
                    builder.Key(k => { k.Min(options.Key.Min); k.Max(options.Key.Max); k.Target(options.Key.Target); });
                    // Detects the presence of an audience in the recording. Higher liveness values represent an increased probability that the track was performed live. A value above 0.8 provides strong likelihood that the track is live.
                    builder.Liveness(l => { l.Min(options.Liveness.Min); l.Max(options.Liveness.Max); l.Target(options.Liveness.Target); });
                    // The overall loudness of a track in decibels (dB). Loudness values are averaged across the entire track and are useful for comparing relative loudness of tracks. Loudness is the quality of a sound that is the primary psychological correlate of physical strength (amplitude). Values typical range between -60 and 0 db.
                    builder.Loudness(l => { l.Min(options.Loudness.Min); l.Max(options.Loudness.Max); l.Target(options.Loudness.Target); });
                    // The popularity of the track. The value will be between 0 and 100, with 100 being the most popular. The popularity is calculated by algorithm and is based, in the most part, on the total number of plays the track has had and how recent those plays are. Note: When applying track relinking via the market parameter, it is expected to find relinked tracks with popularities that do not match min_*, max_*and target_* popularities. These relinked tracks are accurate replacements for unplayable tracks with the expected popularity scores. Original, non-relinked tracks are available via the linked_from attribute of the relinked track response.
                    builder.Popularity(p => { p.Min(options.Popularity.Min); p.Max(options.Popularity.Max); p.Target(options.Popularity.Target); });
                    // Speechiness detects the presence of spoken words in a track. The more exclusively speech-like the recording (e.g. talk show, audio book, poetry), the closer to 1.0 the attribute value. Values above 0.66 describe tracks that are probably made entirely of spoken words. Values between 0.33 and 0.66 describe tracks that may contain both music and speech, either in sections or layered, including such cases as rap music. Values below 0.33 most likely represent music and other non-speech-like tracks.
                    builder.Speechiness(s => { s.Min(options.Speechiness.Min); s.Max(options.Speechiness.Max); s.Target(options.Speechiness.Target); });
                    // The overall estimated tempo of a track in beats per minute (BPM). In musical terminology, tempo is the speed or pace of a given piece and derives directly from the average beat duration.
                    builder.Tempo(t => { t.Min(options.Tempo.Min); t.Max(options.Tempo.Max); t.Target(options.Tempo.Target); });
                    // A measure from 0.0 to 1.0 describing the musical positiveness conveyed by a track. Tracks with high valence sound more positive (e.g. happy, cheerful, euphoric), while tracks with low valence sound more negative (e.g. sad, depressed, angry).
                    builder.Valence(v => { v.Min(options.Valence.Min); v.Max(options.Valence.Max); v.Target(options.Valence.Target); });
                    // Mode indicates the modality (major or minor) of a track, the type of scale from which its melodic content is derived. Major is represented by 1 and minor is 0.
                    builder.Mode(m => { m.Min(options.Mode.Min); m.Max(options.Mode.Max); m.Target(options.Mode.Target); });
                    // An estimated overall time signature of a track. The time signature (meter) is a notational convention to specify how many beats are in each bar (or measure).
                    builder.TimeSignature(t => { t.Min(options.TimeSignature.Min); t.Max(options.TimeSignature.Max); t.Target(options.TimeSignature.Target); });
                });

                var recommendedTracks = (await this.fluentSpotifyClient
                    .Browse
                    .Recommendations
                    .GetAsync(
                        limit: max,
                        market: options.CountryCode.ToString(),
                        seedArtists: recommendation.SeedArtists,
                        seedGenres: recommendation.SeedGenres,
                        seedTracks: recommendation.SeedTracks))
                    .Tracks
                    .Select(item => new Song
                    {
                        Name = item.Name,
                        Artists = item.Artists.Select(artist => artist.Name),
                        Uri = item.Uri.Split(':').Last()
                    })
                    .ToList();

                if (recommendedTracks.Count() == 0)
                {
                    break;
                }

                recommendation.Songs.AddRange(recommendedTracks);
            }

            return recommendation;
        }
    }
}