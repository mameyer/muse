using System.Collections.Generic;

namespace Muse.Models
{
    public enum CountryCode
	{
		None = 0,
		AW,
		AF,
		AO,
		AI,
		AX,
		AL,
		AD,
		AE,
		AR,
		AM,
		AS,
		AQ,
		TF,
		AG,
		AU,
		AT,
		AZ,
		BI,
		BE,
		BJ,
		BF,
		BD,
		BG,
		BH,
		BS,
		BA,
		BL,
		BY,
		BZ,
		BM,
		BO,
		BR,
		BB,
		BN,
		BT,
		BV,
		BW,
		CF,
		CA,
		CC,
		CH,
		CL,
		CN,
		CI,
		CM,
		CD,
		CG,
		CK,
		CO,
		KM,
		CV,
		CR,
		CU,
		CW,
		CX,
		KY,
		CY,
		CZ,
		DE,
		DJ,
		DM,
		DK,
		DO,
		DZ,
		EC,
		EG,
		ER,
		EH,
		ES,
		EE,
		ET,
		FI,
		FJ,
		FK,
		FR,
		FO,
		FM,
		GA,
		GB,
		GE,
		GG,
		GH,
		GI,
		GN,
		GP,
		GM,
		GW,
		GQ,
		GR,
		GD,
		GL,
		GT,
		GF,
		GU,
		GY,
		HK,
		HM,
		HN,
		HR,
		HT,
		HU,
		ID,
		IM,
		IN,
		IO,
		IE,
		IR,
		IQ,
		IS,
		IL,
		IT,
		JM,
		JE,
		JO,
		JP,
		KZ,
		KE,
		KG,
		KH,
		KI,
		KN,
		KR,
		XK,
		KW,
		LA,
		LB,
		LR,
		LY,
		LC,
		LI,
		LK,
		LS,
		LT,
		LU,
		LV,
		MO,
		MF,
		MA,
		MC,
		MD,
		MG,
		MV,
		MX,
		MH,
		MK,
		ML,
		MT,
		MM,
		ME,
		MN,
		MP,
		MZ,
		MR,
		MS,
		MQ,
		MU,
		MW,
		MY,
		YT,
		NA,
		NC,
		NE,
		NF,
		NG,
		NI,
		NU,
		NL,
		NO,
		NP,
		NR,
		NZ,
		OM,
		PK,
		PA,
		PN,
		PE,
		PH,
		PW,
		PG,
		PL,
		PR,
		KP,
		PT,
		PY,
		PS,
		PF,
		QA,
		RE,
		RO,
		RU,
		RW,
		SA,
		SD,
		SN,
		SG,
		GS,
		SJ,
		SB,
		SL,
		SV,
		SM,
		SO,
		PM,
		RS,
		SS,
		ST,
		SR,
		SK,
		SI,
		SE,
		SZ,
		SX,
		SC,
		SY,
		TC,
		TD,
		TG,
		TH,
		TJ,
		TK,
		TM,
		TL,
		TO,
		TT,
		TN,
		TR,
		TV,
		TW,
		TZ,
		UG,
		UA,
		UM,
		UY,
		US,
		UZ,
		VA,
		VC,
		VE,
		VG,
		VI,
		VN,
		VU,
		WF,
		WS,
		YE,
		ZA,
		ZM,
		ZW,
		BQ,
		SH,
		CS
    }

    public class RecommendationOption<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }
        public T Target { get; set; }
    }

    public class RecommendationOptions
    {
        public RecommendationOptions()
        {
            CountryCode = CountryCode.US;

            Acousticness = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Danceability = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            DurationMs = new RecommendationOption<int>
            {
                Target = 180,
                Min = 0,
                Max = 1000
            };
            Energy = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Instrumentalness = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Key = new RecommendationOption<int>
            {
                Target = 5,
                Min = 0,
                Max = 100
            };
            Liveness = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Loudness = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Popularity = new RecommendationOption<int>
            {
                Target = 0,
                Min = 20,
                Max = 100
            };
            Speechiness = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Tempo = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Valence = new RecommendationOption<float>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            Mode = new RecommendationOption<int>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
            TimeSignature = new RecommendationOption<int>
            {
                Target = 0,
                Min = 0,
                Max = 1
            };
        }

        public string[] Artists { get; set; }

        public string[] Tracks { get; set; }

        public string[] Genres { get; set; }

        public RecommendationOption<float> Acousticness { get; set; }
        public RecommendationOption<float> Danceability { get; set; }
        public RecommendationOption<int> DurationMs { get; set; }
        public RecommendationOption<float> Energy { get; set; }
        public RecommendationOption<float> Instrumentalness { get; set; }
        public RecommendationOption<int> Key { get; set; }
        public RecommendationOption<float> Liveness { get; set; }
        public RecommendationOption<float> Loudness { get; set; }
        public RecommendationOption<int> Popularity { get; set; }
        public RecommendationOption<float> Speechiness { get; set; }
        public RecommendationOption<float> Tempo { get; set; }
        public RecommendationOption<float> Valence { get; set; }
        public RecommendationOption<int> Mode { get; set; }
        public RecommendationOption<int> TimeSignature { get; set; }
        public IEnumerable<Models.Local.Song> RecommendedSongs { get; set; }
        public CountryCode CountryCode { get; set; }
    }
}