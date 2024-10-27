var Muse;
(function (Muse) {
    var Views;
    (function (Views) {
        var Player = /** @class */ (function () {
            function Player(window) {
                var _this = this;
                this.useGIF = false;
                this.audioAnalysisSections = null;
                this.loudnessMax = 60;
                window["featureAnalysis"] = function () { return _this.featureAnalysis(); };
                window["updateAudioAnalysisChartConstantLines"] = function () { return _this.updateAudioAnalysisChartConstantLines(null); };
                window["audioAnalysis"] = function () { return _this.audioAnalysis(); };
                window["currentPlayingCallback"] = function () { return _this.currentPlayingCallback(false, null); };
            }
            Player.prototype.getFeaturesAnalysisForm = function () {
                return $("#trackFeatures").dxForm("instance");
            };
            Player.prototype.getAudioAnalysisChart = function () {
                return $("#audioAnalysis").dxChart("instance");
            };
            Player.prototype.featureAnalysis = function () {
                var _this = this;
                $.ajax({
                    url: this.FeatureAnalysisUrl,
                    data: {
                        Id: this.currentTrack.item.id
                    },
                    success: function (result) {
                        _this.getFeaturesAnalysisForm().option("formData", result);
                    }
                });
            };
            Player.prototype.getLoudnessChart = function () {
                return $("#loudness").dxChart("instance");
            };
            Player.prototype.updateAudioAnalysisChartConstantLines = function (diff) {
                var _this = this;
                var audioAnalysisChart = this.getAudioAnalysisChart();
                var constantLines = [];
                if (this.audioAnalysisSections)
                    constantLines = this.audioAnalysisSections;
                var progress = this.currentTrack.progress_ms;
                if (diff)
                    progress += diff;
                var seconds = progress / 1000.0;
                if (this.currentTrack.progress_ms)
                    constantLines = constantLines.concat([{ value: seconds, color: 'red', dashStyle: 'dash', width: 3 }]);
                audioAnalysisChart.option("argumentAxis.constantLines", constantLines);
                var loudnessChart = this.getLoudnessChart();
                var series = audioAnalysisChart.getSeriesByName("segments");
                var filteredPoints = series.getAllPoints().filter(function (e) { return e.data.x_3 < (seconds + 0.5); });
                if (filteredPoints.length > 0) {
                    var point = filteredPoints[filteredPoints.length - 1];
                    point.select();
                    var ps = filteredPoints.slice(Math.max(filteredPoints.length - 25, 0)).map(function (o, index) { return ({ x: index + 1, y: _this.loudnessMax + o.data.y_3 }); });
                    //let s = loudnessChart.getSeriesByName("loudness");
                    //let p0 = s.getAllPoints()[0];
                    //p0.y = point.data.y_3;
                    loudnessChart.option("dataSource", ps);
                    loudnessChart.option("valueAxis.visualRange", [40, this.loudnessMax + 5]);
                }
            };
            Player.prototype.getPitchClass = function (k) {
                switch (k) {
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
            };
            Player.prototype.audioAnalysis = function () {
                var _this = this;
                if (!this.currentTrack)
                    return;
                $.ajax({
                    url: this.AudioAnalysisUrl,
                    data: {
                        Id: this.currentTrack.item.id
                    },
                    success: function (result) {
                        var data = [];
                        var bars = result.bars.map(function (p) { return ({ x_0: p.start, y_0: p.duration }); });
                        var beats = result.beats.map(function (p) { return ({ x_1: p.start, y_1: p.duration }); });
                        var tatums = result.tatums.map(function (p) { return ({ x_2: p.start, y_2: p.duration }); });
                        var segments = result.segments.map(function (p) { return ({ x_3: p.start, y_3: p.loudnessMax }); });
                        var max_tempo = Math.max.apply(Math, result.sections.map(function (p) { return p.tempo; }));
                        var min_tempo = Math.min.apply(Math, result.sections.filter(function (e) { return e.tempo > 0; }).map(function (p) { return p.tempo; }));
                        var tempo_diff = max_tempo - min_tempo;
                        var tempo = [];
                        if (tempo_diff >= 0)
                            tempo = result.sections
                                .map(function (p) { return [
                                { x_4: p.start, y_4: function (o) { if (o.tempo <= 0)
                                        return 0; return (o.tempo - min_tempo) / tempo_diff; }(p) },
                                { x_4: p.start + p.duration, y_4: function (o) { if (o.tempo <= 0)
                                        return 0; return (o.tempo - min_tempo) / tempo_diff; }(p) }
                            ]; })
                                .flat();
                        data = bars.concat(beats).concat(tatums).concat(segments).concat(tempo);
                        var audioAnalysisChart = _this.getAudioAnalysisChart();
                        audioAnalysisChart.option("dataSource", data);
                        var sections = result.sections
                            .map(function (p) { return ({
                            value: p.start,
                            color: "grey",
                            label: { text: "\nk: " + _this.getPitchClass(p.key) + "\nconf: " + p.confidence + "\ntempo: " + p.tempo },
                            width: (function (p) {
                                var sectionWidth = 1;
                                if (p.confidence >= 1)
                                    sectionWidth = 7;
                                else if (p.confidence >= 0.75)
                                    sectionWidth = 6;
                                else if (p.confidence >= 0.5)
                                    sectionWidth = 5;
                                else if (p.confidence >= 0.4)
                                    sectionWidth = 4;
                                else if (p.confidence >= 0.3)
                                    sectionWidth = 3;
                                else if (p.confidence >= 0.2)
                                    sectionWidth = 2;
                                return sectionWidth;
                            })(p)
                        }); });
                        _this.audioAnalysisSections = sections;
                        _this.updateAudioAnalysisChartConstantLines(null);
                    }
                });
            };
            Player.prototype.currentPlayingCallback = function (hasChanged, diff) {
                var result = $("<div>");
                if (!this.currentTrack) {
                    $("#track-info").html("loading..");
                    this.getFeaturesAnalysisForm().option("formData", result);
                    this.getAudioAnalysisChart().option("dataSource", []);
                    return;
                }
                var url = this.currentTrack.item.album.images[0].url;
                if (hasChanged) {
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
                        + "<u>artists:</u> " + this.currentTrack.item.artists.map(function (f) { return f.name; }).join(", ")
                        + "  |  <u>album:</u> " + this.currentTrack.item.album.name + "<p></p>"
                        + "<u>popularity:</u> " + this.currentTrack.item.popularity
                        + "  |  <u>device:</u> " + this.currentTrack.device.name + "<p></p>"
                        + "<div id='artist-info'></div>")
                        .appendTo(result);
                    $("#track-info").html(result);
                    this.featureAnalysis();
                    this.audioAnalysis();
                    this.updateArtistInfo();
                }
                else {
                    //if ((lastUpdate - newSince) > 500) this.updateAudioAnalysisChartConstantLines(diff);
                }
            };
            Player.prototype.updateArtistInfo = function () {
                if (!this.currentTrack) {
                    $("#artist-info").html("");
                    return;
                }
                $.ajax({
                    url: this.LastFMRequestUrl,
                    data: {
                        artist: this.currentTrack.item.artists[0].name,
                        track: this.currentTrack.item.name
                    },
                    success: function (e) {
                        if (!e)
                            return;
                        $("#artist-info").html("<div>"
                            + "<u>bio:</u></br>" + e.Artist.Bio.Summary + "<p></p>"
                            + "<u>similar:</u></br>" + e.Artist.Similar.map(function (f) { return f.Name; }).join(",") + "<p></p>"
                            + "<u>tags:</u></br>" + e.Track.TopTags.map(function (f) { return f.Name; }).join(",")
                            + "</div>");
                    }
                });
            };
            return Player;
        }());
        Views.Player = Player;
    })(Views = Muse.Views || (Muse.Views = {}));
})(Muse || (Muse = {}));
