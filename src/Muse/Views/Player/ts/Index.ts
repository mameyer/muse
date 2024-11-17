namespace muse.views.player
{
    export class Index
    {
        useGIF: boolean = false;
        audioAnalysisSections: any = null;
        loudnessMax: number = 60;
        currentTrack: Player.Interfaces.DTO.ICurrentlyPlayingDTO;
        public FeatureAnalysisUrl: string;
        public AudioAnalysisUrl: string;
        public LastFMRequestUrl: string;

        constructor(window: Window) {
            window["currentPlayingCallback"] = (track, hasChanged, diff) => this.currentPlayingCallback(track, hasChanged, diff);
        }

        getFeaturesAnalysisForm() {
            return ($("#trackFeatures") as any).dxForm("instance");
        }

        getAudioAnalysisChart() {
            return ($("#audioAnalysis") as any).dxChart("instance");
        }

        featureAnalysis() {
            if (this.currentTrack?.item == null) {
                return;
            }

            $.ajax({
                url: this.FeatureAnalysisUrl,
                data: {
                    id: this.currentTrack.item.id
                },
                success: (result) => {
                    this.getFeaturesAnalysisForm().option("formData", result);
                }
            });
        }

        getLoudnessChart() {
            return ($("#loudness") as any).dxChart("instance");
        }

        updateAudioAnalysisChartConstantLines() {
            if (this.currentTrack == null) {
                return;
            }

            let audioAnalysisChart = this.getAudioAnalysisChart();
            let constantLines = [];

            if (this.audioAnalysisSections) {
                constantLines = this.audioAnalysisSections;
            }

            let progressS = this.currentTrack.progressMs / 1000.0;
            console.debug("progress: " + progressS +  "s");

            if (progressS > 0) {
                constantLines = constantLines.concat([ { value: progressS, color: 'red', dashStyle: 'dash', width: 3 } ]);
            }

            audioAnalysisChart.option("argumentAxis.constantLines", constantLines);

	        let loudnessChart = this.getLoudnessChart();

            let series = audioAnalysisChart.getSeriesByName("segments");
            let filteredPoints = series.getAllPoints().filter(e => e.data.x_3 < (progressS + 0.5));
            if (filteredPoints.length > 0) {
                let point = filteredPoints[filteredPoints.length - 1];
                point.select();

                let ps = filteredPoints.slice(Math.max(filteredPoints.length - 25, 0)).map((o, index) => ({ x: index +1, y: this.loudnessMax +  o.data.y_3 }));

	            loudnessChart.option("dataSource", ps);
                loudnessChart.option("valueAxis.visualRange", [40, this.loudnessMax+ 5]);
            }
        }

        getPitchClass(k) {
            switch(k) {
                case 0:
                    return "C";

                case 1:
                    return "C#/Db";

                case 2:
                    return "D";

                case 3:
                    return "D#/Eb";

                case 4:
                    return "E";

                case 5:
                    return "E#/Fb";

                case 6:
                    return "F";

                case 7:
                    return "F#/Gb";

                case 8:
                    return "G";

                case 9:
                    return "G#/Ab";

                case 10:
                    return "A";

                case 11:
                    return "A#/B";

                default:
                    return "/";
            }
        }

        async audioAnalysis() {
            if (this.currentTrack?.item == null) {
                return;
            }

            return $.ajax({
                url: this.AudioAnalysisUrl,
                data: {
                    Id: this.currentTrack.item.id
                },
                success: (result) => {
                    let data = [];

                    let bars = result.bars.map(p => ({ x_0: p.start, y_0: p.duration }));
                    let beats = result.beats.map(p => ({ x_1: p.start, y_1: p.duration }));
                    let tatums = result.tatums.map(p => ({ x_2: p.start, y_2: p.duration }));
                    let segments = result.segments.map(p => ({ x_3: p.start, y_3: p.loudnessMax }));

                    let max_tempo = Math.max.apply(Math, result.sections.map(function(p) { return p.tempo; }));
                    let min_tempo = Math.min.apply(Math, result.sections.filter(e => e.tempo > 0).map(function(p) { return p.tempo; }));
                    
                    let tempo_diff = max_tempo - min_tempo;
                    let tempo = [];
                    if (tempo_diff >= 0) tempo = result.sections
                        .map(p => [ 
                            { x_4: p.start, y_4: function(o) { if (o.tempo <= 0) return 0; return (o.tempo - min_tempo) / tempo_diff; }(p) },
                            { x_4: p.start + p.duration, y_4: function(o) { if (o.tempo <= 0) return 0; return (o.tempo - min_tempo) / tempo_diff; }(p) }
                            ])
                        .flat();

                    data = bars.concat(beats).concat(tatums).concat(segments).concat(tempo);

                    let audioAnalysisChart = this.getAudioAnalysisChart();
                    audioAnalysisChart.option("dataSource", data);

                    let sections = result.sections
                        .map(p => ({
                            value: p.start,
                            color: "grey",
                            label: { text: "\nk: " + this.getPitchClass(p.key) + "\nconf: " + p.confidence + "\ntempo: " + p.tempo },
                            width: ((p) => {
                                let sectionWidth = 1;
                                if (p.confidence >= 1) sectionWidth = 7;
                                else if (p.confidence >= 0.75) sectionWidth = 6;
                                else if (p.confidence >= 0.5) sectionWidth = 5;
                                else if (p.confidence >= 0.4) sectionWidth = 4;
                                else if (p.confidence >= 0.3) sectionWidth = 3;
                                else if (p.confidence >= 0.2) sectionWidth = 2;
                                return sectionWidth;
                                })(p)
                            }));

                    this.audioAnalysisSections = sections;
                }
            });
        }

        currentPlayingCallback(track, hasChanged, diff) {
            this.currentTrack = track;

            let result = $("<div>");
            if (this.currentTrack?.item == null) {
                $("#track-info").html("loading..");
                this.getFeaturesAnalysisForm().option("formData", result);
                this.getAudioAnalysisChart().option("dataSource", []);
                return;
            }

            if (hasChanged) {
                let url = "";
                if (this.currentTrack.item.album?.images != null
                    && this.currentTrack.item.album?.images.length > 0) {
                    url = this.currentTrack.item.album?.images[0].url;
                }

                $("<img>")
                    .css("float", "left")
                    .css("margin-top", "20px")
                    .attr("height", 350)
                    .attr("width", 350)
                    .attr("src", url)
                    .appendTo(result);
                $("<div>")
                    .css("float", "left")
                    .css("width", "calc(100% - 400px)")
                    .css("margin-left", "20px")
                    .html("<h4>" + this.currentTrack.item.name + "</h4><p></p>"
                        + "<u>artists:</u> " + this.currentTrack.item.artists.map(f => f.name).join(", ") 
                        + "  |  <u>album:</u> " + this.currentTrack.item.album?.name + "<p></p>"
                        + "<u>popularity:</u> " + this.currentTrack.item.popularity
                        + "  |  <u>device:</u> " + this.currentTrack.device?.name + "<p></p>"
                        + "<div id='artist-info'></div>")
                    .appendTo(result);
                $("#track-info").html(result as any);

                this.featureAnalysis();
                this.audioAnalysis()
                    .then((e) => {
                        this.updateAudioAnalysisChartConstantLines();
                    });

                this.updateArtistInfo();
            } else {
                 this.updateAudioAnalysisChartConstantLines();
            }
        }

        updateArtistInfo() {
            if (this.currentTrack?.item?.artists == null) {
                $("#artist-info").html("");
                return;
            }

            $.ajax({
                url: this.LastFMRequestUrl,
                data: {
                    artist: this.currentTrack.item.artists[0].name,
                    track: this.currentTrack.item.name
                },
                success: (e) => {
                    if (!e) {
                        return;
                    }

                    $("#artist-info").html("<div>"
                        + "<u>bio:</u></br>" + e.artist?.bio?.summary + "<p></p>"
                        + "<u>similar:</u></br>" + e.artist?.similar?.map(f => f.name).join(",") + "<p></p>"
                        + "<u>tags:</u></br>" + e.track?.topTags?.map(f => f.name).join(",")
                        + "</div>");
                }
            });
        }
    }
}