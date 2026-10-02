namespace OpenATO.Configuration
{
    public sealed class SpeedLimitPoint
    {
        private const double MphToKph = 1.609344;

        public double PositionMetres { get; }
        public double SpeedMph { get; }

        public double SpeedKph
        {
            get
            {
                return SpeedMph * MphToKph;
            }
        }

        public SpeedLimitPoint(
            double positionMetres,
            double speedMph)
        {
            PositionMetres = positionMetres;
            SpeedMph = speedMph;
        }
    }

    public sealed class StationProfile
    {
        public string Name { get; }

        public double PlatformStartMetres { get; }

        public double StopPositionMetres { get; }

        /*
         * Provisional target speed that ATO should reach
         * before the front enters the platform.
         */
        public double PlatformEntrySpeedKph { get; }

        public StationProfile(
            string name,
            double platformStartMetres,
            double stopPositionMetres,
            double platformEntrySpeedKph)
        {
            Name = name;
            PlatformStartMetres = platformStartMetres;
            StopPositionMetres = stopPositionMetres;
            PlatformEntrySpeedKph = platformEntrySpeedKph;
        }
    }

    public static class JubileeRouteProfile
    {
        /*
         * These values are written in mph in the route.
         * SpeedLimitPoint converts them to km/h when
         * SpeedKph is requested.
         */
        public static readonly SpeedLimitPoint[] SpeedLimits =
        {
    new SpeedLimitPoint(325.0, 20.0),

    /*
     * Correct operational line speed leaving Stratford.
     * The raw route's 65 mph command at 625 m is ignored.
     */
    new SpeedLimitPoint(488.0, 60.0),

    new SpeedLimitPoint(1200.0, 40.0),
            new SpeedLimitPoint(1904.7, 35.0),
            new SpeedLimitPoint(2306.0, 80.0),
            new SpeedLimitPoint(3170.0, 35.0),
            new SpeedLimitPoint(3450.0, 80.0),
            new SpeedLimitPoint(4975.0, 50.0),
            new SpeedLimitPoint(5290.0, 80.0),
            new SpeedLimitPoint(5600.0, 80.0),
            new SpeedLimitPoint(7675.0, 80.0),
            new SpeedLimitPoint(12570.0, 42.0),
            new SpeedLimitPoint(12715.0, 80.0),
            new SpeedLimitPoint(13975.0, 47.0),
            new SpeedLimitPoint(14430.0, 80.0),
            new SpeedLimitPoint(16682.8, 37.0),
            new SpeedLimitPoint(16992.0, 45.0),
            new SpeedLimitPoint(17760.0, 80.0),
            new SpeedLimitPoint(18425.0, 40.0),
            new SpeedLimitPoint(18660.0, 80.0),
            new SpeedLimitPoint(19000.0, 45.0),
            new SpeedLimitPoint(20155.0, 35.0),
            new SpeedLimitPoint(20478.0, 31.5),
            new SpeedLimitPoint(20691.0, 80.0),
            new SpeedLimitPoint(23000.0, 40.0),
            new SpeedLimitPoint(23336.0, 38.0),
            new SpeedLimitPoint(23965.0, 50.0)
        };

        /*
         * Begin with West Ham only.
         *
         * This lets us develop and test one complete
         * station approach before adding every station.
         */
        public static readonly StationProfile[] Stations =
        {
            new StationProfile(
                name: "West Ham",
                platformStartMetres: 1750.0,
                stopPositionMetres: 1896.88,
                platformEntrySpeedKph: 40.0 * 1.609344),

                    new StationProfile(
        name: "Canning Town",
        platformStartMetres: 3303.6,
        stopPositionMetres: 3440.55,
        platformEntrySpeedKph: 35.0 * 1.609344),

                new StationProfile(
    name: "North Greenwich",
    platformStartMetres: 5107.28,
    stopPositionMetres: 5279.46,
    platformEntrySpeedKph: 45.0 * 1.609344),

                new StationProfile(
    name: "Canary Wharf",
    platformStartMetres: 6815.3,
    stopPositionMetres: 7179.24,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Canada Water",
    platformStartMetres: 9283.9,
    stopPositionMetres: 9679.21,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Bermondsey",
    platformStartMetres: 10463.5,
    stopPositionMetres: 10754.20,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "London Bridge",
    platformStartMetres: 12568.41,
    stopPositionMetres: 12704.0,
    platformEntrySpeedKph: 47.0 * 1.609344),

                new StationProfile(
    name: "Southwark",
    platformStartMetres: 13643.8,
    stopPositionMetres: 13954.22, 
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Waterloo",
    platformStartMetres: 14210.6,
    stopPositionMetres: 14404.20,
    platformEntrySpeedKph: 47.0 * 1.609344),

                new StationProfile(
    name: "Westminster",
    platformStartMetres: 15144.65,
    stopPositionMetres: 15404.20,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Green Park",
    platformStartMetres: 16837.0,
    stopPositionMetres: 16980.24,
    platformEntrySpeedKph: 37.0 * 1.609344),

                new StationProfile(
    name: "Bond Street",
    platformStartMetres: 18150.5,
    stopPositionMetres: 18405.30,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Baker Street",
    platformStartMetres: 19977.7,
    stopPositionMetres: 20144.00,
    platformEntrySpeedKph: 45.0 * 1.609344),

                new StationProfile(
    name: "St John's Wood",
    platformStartMetres: 22002.4,
    stopPositionMetres: 22293.0,
    platformEntrySpeedKph: 80.0 * 1.609344),

                new StationProfile(
    name: "Swiss Cottage",
    platformStartMetres: 23134.0,
    stopPositionMetres: 23269.70,
    platformEntrySpeedKph: 40.0 * 1.609344),

                new StationProfile(
    name: "Finchley Road",
    platformStartMetres: 23791.6,
    stopPositionMetres: 23959.6,
    platformEntrySpeedKph: 38.0 * 1.609344),

                new StationProfile(
    name: "West Hampstead",
    platformStartMetres: 24312.0,
    stopPositionMetres: 24581.50,
    platformEntrySpeedKph: 50.0 * 1.609344)
        };

        public static double GetCurrentSpeedLimitKph(
            double locationMetres)
        {
            /*
             * Before the first explicit limit at 325 m,
             * use that first limit as the safe default.
             */
            double currentLimitKph =
                SpeedLimits[0].SpeedKph;

            for (int i = 0; i < SpeedLimits.Length; i++)
            {
                if (locationMetres <
                    SpeedLimits[i].PositionMetres)
                {
                    break;
                }

                currentLimitKph =
                    SpeedLimits[i].SpeedKph;
            }

            return currentLimitKph;
        }

        public static SpeedLimitPoint GetNextSpeedLimit(
            double locationMetres)
        {
            for (int i = 0; i < SpeedLimits.Length; i++)
            {
                if (SpeedLimits[i].PositionMetres >
                    locationMetres)
                {
                    return SpeedLimits[i];
                }
            }

            return null;
        }

        public static StationProfile GetNextStation(
            double locationMetres)
        {
            for (int i = 0; i < Stations.Length; i++)
            {
                if (Stations[i].StopPositionMetres >
                    locationMetres)
                {
                    return Stations[i];
                }
            }

            return null;
        }
    }
}