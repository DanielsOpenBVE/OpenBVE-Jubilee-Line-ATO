using System;

namespace OpenATO.Configuration
{
    public static class JubileeRouteSupervisor
    {
        private const double DeepLevelStartMetres = 3450.0;
        private const double DeepLevelB3DecelerationMps2 = 1.32;
        private const double DeepLevelB3StartMarginMetres = 0.0;
        private const double DeepLevelB3ReleaseToleranceKph = 5.0;
        /*
         * 1 km/h buffer so the low-level controller's
         * normal tolerance does not carry the train
         * above a hard route speed limit.
         */
        private const double WestHamSpeedLimitBufferKph = 1.0;

        private const double WestHam40MphLimitStartMetres = 1200.0;

        private const double WestHam40MphLimitEndMetres = 1904.7;

        /*
         * Used to calculate advance braking curves
         * for upcoming lower speed limits and station
         * approach speeds.
         */
        private const double ApproachDecelerationMps2 = 0.45;

        /*
         * These are retained for other stations when
         * adaptive station braking is added.
         */
        private const double StationB3ThresholdMps2 = 0.85;
        private const double StationB2ThresholdMps2 = 0.45;

        private const double WestHamInitialB3DistanceMetres = 53.6;

        /*
         * Total cycle length for blended B2/B3 braking.
         * 0.4 seconds = 0.2 s B3 + 0.2 s B2.
         */
        private const double WestHamBlendCycleSeconds = 4.0;

        /*
         * 0.50 means B3 is used for 50% of each cycle.
         */
        private const double WestHamB3BlendFraction = 0.30;

        private const double MarkerCaptureDistanceMetres = 8.0;
        private const double ExtendedMarkerCaptureDistanceMetres = 20.0;

        private const double MarkerCaptureB2ThresholdMps2 = 0.45;

        private const double MarkerCaptureB3ThresholdMps2 = 0.85;

        private static double westHamBlendTimerSeconds = 0.0;
        private static bool westHamStopCompleted = false;

        private const double CanningTownInitialB2DistanceMetres = 30.0;

        private const double CanningTownBlendCycleSeconds = 4.0;

        private const double CanningTownB3BlendFraction = 0.30;

        private static double canningTownBlendTimerSeconds = 0.0;

        private static bool canningTownStopCompleted = false;

        private static bool northGreenwichStopCompleted = false;

        private static bool canaryWharfStopCompleted = false;

        private static bool canadaWaterStopCompleted = false;

        private static bool bermondseyStopCompleted = false;

        private static bool londonBridgeStopCompleted = false;

        private const double LondonBridgeCoastStartMetres = 11925.0;

        private const double LondonBridge40LimitMetres = 12570.0;

        private const double LondonBridgeStationBrakeStartMetres = 12575.0;

        private const double LondonBridge40Kph =
            42.0 * 1.609344;

        private const double LondonBridgeB1TriggerDecelerationMps2 = 0.35;

        private const double LondonBridgeB1ReleaseToleranceKph = 0.8;

        private static bool londonBridgeB1Active = false;

        private static bool southwarkStopCompleted = false;

        private static bool waterlooStopCompleted = false;

        private static bool westminsterStopCompleted = false;

        private const double GreenParkInitialB2DistanceMetres = 115.3;
        private static bool greenParkStopCompleted = false;

        private const double GreenPark35LimitMetres = 16682.8;

        private const double GreenPark35Kph =
            37.0 * 1.609344;

        private const double GreenParkBrakeTriggerDecelerationMps2 = 1.10;

        private const double GreenParkBrakeReleaseToleranceKph = 1.0;

        private static bool greenParkSpeedLimitB3Active = false;

        private static bool bondStreetStopCompleted = false;

        private static bool bakerStreetStopCompleted = false;

        private static bool stJohnsWoodStopCompleted = false;

        private static bool swissCottageStopCompleted = false;

        private const double SwissCottage40LimitMetres = 23000.0;

        private const double SwissCottage40Kph =
            40.0 * 1.609344;

        /*
         * First-test values:
         *
         * Start B2 330 m before the restriction.
         * Upgrade to B3 for the final 120 m.
         */
        private const double SwissCottage40B2StartDistanceMetres = 330.0;
        private const double SwissCottage40B3StartDistanceMetres = 120.0;

        private const double SwissCottage40ReleaseToleranceKph = 1.0;

        private const double FinchleyRoad38LimitMetres = 23336.0;

        private const double FinchleyRoad38Kph =
            38.0 * 1.609344;

        private const double FinchleyRoad38B2StartDistanceMetres = 30.0;

        private const double FinchleyRoad38ReleaseToleranceKph = 1.0;

        private static bool finchleyRoadStopCompleted = false;

        private const double FinchleyRoadInitialB1DistanceMetres = 28.0;

        private static bool westHampsteadStopCompleted = false;

        private static double CalculateB3BrakingDistanceMetres(
    double initialSpeedKph,
    double targetSpeedKph)
        {
            if (initialSpeedKph <= targetSpeedKph)
            {
                return 0.0;
            }

            double initialSpeedMps =
                initialSpeedKph / 3.6;

            double targetSpeedMps =
                targetSpeedKph / 3.6;

            return
                ((initialSpeedMps * initialSpeedMps) -
                 (targetSpeedMps * targetSpeedMps)) /
                (2.0 * DeepLevelB3DecelerationMps2);
        }

        public static double GetTargetSpeedKph(
            double locationMetres)
        {
            double currentLimitKph =
                JubileeRouteProfile.GetCurrentSpeedLimitKph(
                    locationMetres);

            bool inWestHam40MphSection =
     locationMetres >= WestHam40MphLimitStartMetres &&
     locationMetres < WestHam40MphLimitEndMetres;

            double currentLimitBufferKph =
                inWestHam40MphSection
                    ? WestHamSpeedLimitBufferKph
                    : 0.0;

            double bufferedCurrentLimitKph =
                Math.Max(
                    0.0,
                    currentLimitKph - currentLimitBufferKph);

            /*
             * No temporary 75 km/h ceiling anymore.
             * The ATO may use the full route speed.
             */
            double targetKph =
                bufferedCurrentLimitKph;


            /*
             * Look at the immediately upcoming speed
             * restriction. If it is lower than the
             * current limit, calculate an advance
             * braking curve.
             */
            SpeedLimitPoint nextLowerLimit =
                GetNextLowerSpeedLimit(
                    locationMetres,
                    currentLimitKph);

            if (nextLowerLimit != null)
            {
                double distanceToLimitMetres =
                    nextLowerLimit.PositionMetres -
                    locationMetres;

                /*
                 * Preserve the special 1 km/h buffer
                 * for the West Ham 40 mph restriction.
                 */
                bool approachingWestHam40MphLimit =
                    Math.Abs(
                        nextLowerLimit.PositionMetres -
                        WestHam40MphLimitStartMetres) < 0.01;

                double nextLimitBufferKph =
                    approachingWestHam40MphLimit
                        ? WestHamSpeedLimitBufferKph
                        : 0.0;

                double bufferedNextLimitKph =
                    Math.Max(
                        0.0,
                        nextLowerLimit.SpeedKph -
                        nextLimitBufferKph);


                if (locationMetres >= DeepLevelStartMetres)
                {
                    /*
                     * DEEP-LEVEL SECTION
                     *
                     * Calculate one fixed B3 braking point.
                     */
                    bool isSwissCottage40Limit =
                        Math.Abs(
                            nextLowerLimit.PositionMetres -
                            SwissCottage40LimitMetres) < 0.01;

                    bool isFinchleyRoad38Limit =
    Math.Abs(
        nextLowerLimit.PositionMetres -
        FinchleyRoad38LimitMetres) < 0.01;

                    if (isSwissCottage40Limit)
                    {
                        if (distanceToLimitMetres <=
                            SwissCottage40B2StartDistanceMetres)
                        {
                            targetKph =
                                Math.Min(
                                    targetKph,
                                    bufferedNextLimitKph);
                        }
                    }
                    else if (isFinchleyRoad38Limit)
                    {
                        /*
                         * Finchley Road surface-style reduction.
                         * Change the target when B2 begins.
                         */
                        if (distanceToLimitMetres <=
                            FinchleyRoad38B2StartDistanceMetres)
                        {
                            targetKph =
                                Math.Min(
                                    targetKph,
                                    bufferedNextLimitKph);
                        }
                    }
                    else
                    {
                        double b3BrakingDistanceMetres =
                            CalculateB3BrakingDistanceMetres(
                                bufferedCurrentLimitKph,
                                bufferedNextLimitKph);

                        b3BrakingDistanceMetres +=
                            DeepLevelB3StartMarginMetres;

                        if (distanceToLimitMetres <=
                            b3BrakingDistanceMetres)
                        {
                            targetKph =
                                Math.Min(
                                    targetKph,
                                    bufferedNextLimitKph);
                        }
                    }
                }
                else
                {
                    /*
                     * SURFACE / WEST HAM SECTION
                     *
                     * Keep the original gradual braking curve.
                     */
                    double permittedApproachSpeedKph =
                        CalculateApproachSpeedKph(
                            bufferedNextLimitKph,
                            distanceToLimitMetres);

                    targetKph =
                        Math.Min(
                            targetKph,
                            permittedApproachSpeedKph);
                }
            }


            /*
             * Station approach supervision.
             *
             * This controls the speed approaching the
             * platform. The actual station brake notch
             * is selected separately below.
             */
            StationProfile nextStation =
                JubileeRouteProfile.GetNextStation(
                    locationMetres);

            if (nextStation != null)
            {
                double distanceToPlatformStartMetres =
                    nextStation.PlatformStartMetres -
                    locationMetres;

                if (distanceToPlatformStartMetres > 0.0)
                {
                    double permittedStationApproachKph =
                        CalculateApproachSpeedKph(
                            nextStation.PlatformEntrySpeedKph,
                            distanceToPlatformStartMetres);

                    targetKph =
                        Math.Min(
                            targetKph,
                            permittedStationApproachKph);
                }
                else
                {
                    targetKph =
                        Math.Min(
                            targetKph,
                            nextStation.PlatformEntrySpeedKph);
                }
            }

            return Math.Max(
                0.0,
                targetKph);
        }


        private static SpeedLimitPoint GetNextLowerSpeedLimit(
            double locationMetres,
            double currentLimitKph)
        {
            /*
             * If a lower speed limit lies beyond the stop
             * marker of the next station, do not anticipate
             * that restriction before stopping at the station.
             *
             * It will become active normally after departure.
             */
            StationProfile nextStation =
                JubileeRouteProfile.GetNextStation(
                    locationMetres);

            for (int i = 0;
                 i < JubileeRouteProfile.SpeedLimits.Length;
                 i++)
            {
                SpeedLimitPoint point =
                    JubileeRouteProfile.SpeedLimits[i];

                if (point.PositionMetres <= locationMetres)
                {
                    continue;
                }

                /*
                 * Only examine the immediately upcoming
                 * speed-limit change.
                 */
                if (point.SpeedKph < currentLimitKph)
                {
                    /*
                     * Station takes priority if the restriction
                     * itself begins after the station stop marker.
                     */
                    if (nextStation != null &&
                        point.PositionMetres >
                        nextStation.StopPositionMetres)
                    {
                        return null;
                    }

                    return point;
                }

                return null;
            }

            return null;
        }


        public static int GetStationBrakeNotch(
            double locationMetres,
            double speedKph,
            double elapsedSeconds,
            bool doorsClosed)
        {
            if (locationMetres > 1950.0)
            {
                westHamStopCompleted = false;
            }

            if (locationMetres > 3500.0)
            {
                canningTownStopCompleted = false;
            }

            if (locationMetres > 5350.0)
            {
                northGreenwichStopCompleted = false;
            }

            if (locationMetres > 7250.0)
            {
                canaryWharfStopCompleted = false;
            }

            if (locationMetres > 9750.0)
            {
                canadaWaterStopCompleted = false;
            }

            if (locationMetres > 10825.0)
            {
                bermondseyStopCompleted = false;
            }

            if (locationMetres > 12775.0)
            {
                londonBridgeStopCompleted = false;
            }

            if (locationMetres > 14025.0)
            {
                southwarkStopCompleted = false;
            }

            if (locationMetres > 14475.0)
            {
                waterlooStopCompleted = false;
            }

            if (locationMetres > 15475.0)
            {
                westminsterStopCompleted = false;
            }

            if (locationMetres > 17050.0)
            {
                greenParkStopCompleted = false;
            }

            if (locationMetres > 18475.0)
            {
                bondStreetStopCompleted = false;
            }

            if (locationMetres > 20215.0)
            {
                bakerStreetStopCompleted = false;
            }

            if (locationMetres > 22365.0)
            {
                stJohnsWoodStopCompleted = false;
            }

            if (locationMetres > 23340.0)
            {
                swissCottageStopCompleted = false;
            }

            if (locationMetres > 24030.0)
            {
                finchleyRoadStopCompleted = false;
            }

            for (int i = 0;
                 i < JubileeRouteProfile.Stations.Length;
                 i++)
            {
                StationProfile station =
                    JubileeRouteProfile.Stations[i];

                bool frontHasEnteredPlatform =
                    locationMetres >=
                    station.PlatformStartMetres;

                bool stillInStationStoppingZone =
                    locationMetres <=
                    station.StopPositionMetres + 5.0;

                if (!frontHasEnteredPlatform ||
                    !stillInStationStoppingZone)
                {
                    continue;
                }

                bool isWestHam =
                    string.Equals(
                        station.Name,
                        "West Ham",
                        StringComparison.OrdinalIgnoreCase);

                bool isCanningTown =
    string.Equals(
        station.Name,
        "Canning Town",
        StringComparison.OrdinalIgnoreCase);

                bool isNorthGreenwich =
    string.Equals(
        station.Name,
        "North Greenwich",
        StringComparison.OrdinalIgnoreCase);

                bool isCanaryWharf =
    string.Equals(
        station.Name,
        "Canary Wharf",
        StringComparison.OrdinalIgnoreCase);

                bool isCanadaWater =
    string.Equals(
        station.Name,
        "Canada Water",
        StringComparison.OrdinalIgnoreCase);

                bool isBermondsey =
    string.Equals(
        station.Name,
        "Bermondsey",
        StringComparison.OrdinalIgnoreCase);

                bool isLondonBridge =
    string.Equals(
        station.Name,
        "London Bridge",
        StringComparison.OrdinalIgnoreCase);

                bool isSouthwark =
    string.Equals(
        station.Name,
        "Southwark",
        StringComparison.OrdinalIgnoreCase);

                bool isWaterloo =
    string.Equals(
        station.Name,
        "Waterloo",
        StringComparison.OrdinalIgnoreCase);

                bool isWestminster =
    string.Equals(
        station.Name,
        "Westminster",
        StringComparison.OrdinalIgnoreCase);

                bool isGreenPark =
    string.Equals(
        station.Name,
        "Green Park",
        StringComparison.OrdinalIgnoreCase);

                bool isBondStreet =
    string.Equals(
        station.Name,
        "Bond Street",
        StringComparison.OrdinalIgnoreCase);

                bool isBakerStreet =
    string.Equals(
        station.Name,
        "Baker Street",
        StringComparison.OrdinalIgnoreCase);

                bool isStJohnsWood =
    string.Equals(
        station.Name,
        "St John's Wood",
        StringComparison.OrdinalIgnoreCase);

                bool isSwissCottage =
    string.Equals(
        station.Name,
        "Swiss Cottage",
        StringComparison.OrdinalIgnoreCase);

                bool isFinchleyRoad =
    string.Equals(
        station.Name,
        "Finchley Road",
        StringComparison.OrdinalIgnoreCase);

                bool isWestHampstead =
    string.Equals(
        station.Name,
        "West Hampstead",
        StringComparison.OrdinalIgnoreCase);

                /*
                 * WEST HAM EXCEPTION
                 *
                 * Continuous B2 from the configured
                 * platform/braking start position.
                 *
                 * No B3 main phase.
                 * No B3 within one metre of the marker.
                 * B2 remains applied while stopped.
                 */

                if (isWestHam)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        westHamStopCompleted = true;
                        westHamBlendTimerSeconds = 0.0;
                        return 0;
                    }

                    if (westHamStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isCanningTown)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        canningTownStopCompleted = true;
                        canningTownBlendTimerSeconds = 0.0;
                        return 0;
                    }

                    if (canningTownStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isNorthGreenwich)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        northGreenwichStopCompleted = true;
                        return 0;
                    }

                    if (northGreenwichStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isCanaryWharf)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        canaryWharfStopCompleted = true;
                        return 0;
                    }

                    if (canaryWharfStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isCanadaWater)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        canadaWaterStopCompleted = true;
                        return 0;
                    }

                    if (canadaWaterStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isBermondsey)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        bermondseyStopCompleted = true;
                        return 0;
                    }

                    if (bermondseyStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isLondonBridge)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        londonBridgeStopCompleted = true;
                        return 0;
                    }

                    if (londonBridgeStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isSouthwark)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        southwarkStopCompleted = true;
                        return 0;
                    }

                    if (southwarkStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isWaterloo)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        waterlooStopCompleted = true;
                        return 0;
                    }

                    if (waterlooStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isWestminster)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        westminsterStopCompleted = true;
                        return 0;
                    }

                    if (westminsterStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isGreenPark)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        greenParkStopCompleted = true;
                        return 0;
                    }

                    if (greenParkStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isBondStreet)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        bondStreetStopCompleted = true;
                        return 0;
                    }

                    if (bondStreetStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isBakerStreet)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        bakerStreetStopCompleted = true;
                        return 0;
                    }

                    if (bakerStreetStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isStJohnsWood)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        stJohnsWoodStopCompleted = true;
                        return 0;
                    }

                    if (stJohnsWoodStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isSwissCottage)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        swissCottageStopCompleted = true;
                        return 0;
                    }

                    if (swissCottageStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isFinchleyRoad)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        finchleyRoadStopCompleted = true;
                        return 0;
                    }

                    if (finchleyRoadStopCompleted)
                    {
                        return 0;
                    }
                }

                if (isWestHampstead)
                {
                    if (!doorsClosed && speedKph <= 0.5)
                    {
                        westHampsteadStopCompleted = true;
                        return 0;
                    }

                    if (westHampsteadStopCompleted)
                    {
                        return 0;
                    }
                }

                /*
                 * UNIVERSAL MARKER CAPTURE
                 * Applies to West Ham, Canning Town,
                 * and all future stations.
                 */
                double markerCaptureDistanceMetres =
                    (isSouthwark ||
                     isWaterloo ||
                     isGreenPark ||
                     isBondStreet ||
                     isBakerStreet ||
                     isStJohnsWood ||
                     isSwissCottage ||
                     isFinchleyRoad ||
                     isWestHampstead)
                        ? ExtendedMarkerCaptureDistanceMetres
                        : MarkerCaptureDistanceMetres;

                int markerCaptureBrakeNotch =
                    GetMarkerCaptureBrakeNotch(
                        station.StopPositionMetres,
                        locationMetres,
                        speedKph,
                        markerCaptureDistanceMetres);

                if (markerCaptureBrakeNotch >= 0)
                {
                    return markerCaptureBrakeNotch;
                }

                if (isGreenPark)
                {
                    double distanceIntoBrakingZone =
                        locationMetres -
                        station.PlatformStartMetres;

                    /*
                     * First 15 m: B2
                     */
                    if (distanceIntoBrakingZone <
                        GreenParkInitialB2DistanceMetres)
                    {
                        return 2;
                    }

                    /*
                     * After first 15 m: continuous B3
                     */
                    return 3;
                }

                /*
 * CANARY WHARF NORMAL BRAKING
 *
 * Continuous B3 from the configured
 * braking start until marker capture.
 */
                if (isCanaryWharf)
                {
                    return 3;
                }

                /*
 * NORTH GREENWICH NORMAL BRAKING
 *
 * Continuous B3 until the universal
 * marker-capture zone takes over.
 */
                if (isNorthGreenwich)
                {
                    return 3;
                }

                if (isCanadaWater)
                {
                    return 3;
                }

                if (isBermondsey)
                {
                    return 3;
                }

                if (isLondonBridge)
                {
                    return 3;
                }

                if (isSouthwark)
                {
                    return 3;
                }

                if (isWaterloo)
                {
                    return 3;
                }

                if (isWestminster)
                {
                    return 3;
                }

                if (isBondStreet)
                {
                    return 3;
                }

                if (isBakerStreet)
                {
                    return 3;
                }

                if (isStJohnsWood)
                {
                    return 3;
                }

                if (isSwissCottage)
                {
                    return 3;
                }

                /*
 * FINCHLEY ROAD SURFACE BRAKING
 *
 * Continuous B2 rather than deep-level B3.
 */
                if (isFinchleyRoad)
                {
                    double distanceIntoBrakingZone =
                        locationMetres -
                        station.PlatformStartMetres;

                    /*
                     * First 15 metres: B1
                     */
                    if (distanceIntoBrakingZone <
                        FinchleyRoadInitialB1DistanceMetres)
                    {
                        return 1;
                    }

                    /*
                     * After first 15 metres: constant B2
                     * until universal marker capture takes over.
                     */
                    return 2;
                }

                /*
 * WEST HAMPSTEAD SURFACE BRAKING
 *
 * Constant B2 until the final
 * 20 m marker capture takes over.
 */
                if (isWestHampstead)
                {
                    return 2;
                }

                /*
                 * WEST HAM NORMAL BRAKING
                 */
                if (isWestHam)
                {
                    double distanceIntoPlatformMetres =
                        locationMetres -
                        station.PlatformStartMetres;

                    if (distanceIntoPlatformMetres <=
                        WestHamInitialB3DistanceMetres)
                    {
                        westHamBlendTimerSeconds = 0.0;
                        return 3;
                    }


                    /*
                     * Once stopped, hold B2 rather than continuing
                     * to alternate between B2 and B3.
                     */
                    if (speedKph <= 0.5)
                    {
                        westHamBlendTimerSeconds = 0.0;
                        return 2;
                    }

                    /*
                     * After the initial B3 section:
                     * rapidly alternate B2 and B3 to produce an
                     * effective intermediate braking effort.
                     */
                    westHamBlendTimerSeconds +=
                        Math.Max(0.0, elapsedSeconds);

                    double phase =
                        westHamBlendTimerSeconds %
                        WestHamBlendCycleSeconds;

                    double b3Duration =
                        WestHamBlendCycleSeconds *
                        WestHamB3BlendFraction;

                    double b2Duration =
                        WestHamBlendCycleSeconds -
                        b3Duration;

                    /*
                     * Spend most of each cycle in B2.
                     *
                     * 3.0 second cycle:
                     * 2.1 seconds B2
                     * 0.9 seconds B3
                     */
                    if (phase < b2Duration)
                    {
                        return 2;
                    }

                    return 3;
                }

                if (isCanningTown)
                {
                    double distanceIntoPlatformMetres =
                        locationMetres -
                        station.PlatformStartMetres;

                    /*
                     * Canning Town:
                     * first 30 metres use continuous B2.
                     */
                    if (distanceIntoPlatformMetres <=
                        CanningTownInitialB2DistanceMetres)
                    {
                        canningTownBlendTimerSeconds = 0.0;
                        return 2;
                    }

                    /*
                     * Once stopped, hold B2.
                     */
                    if (speedKph <= 0.5)
                    {
                        canningTownBlendTimerSeconds = 0.0;
                        return 2;
                    }

                    /*
                     * After the first 30 metres:
                     * same 70% B2 / 30% B3 blend as West Ham.
                     */
                    canningTownBlendTimerSeconds +=
                        Math.Max(0.0, elapsedSeconds);

                    double phase =
                        canningTownBlendTimerSeconds %
                        CanningTownBlendCycleSeconds;

                    double b3Duration =
                        CanningTownBlendCycleSeconds *
                        CanningTownB3BlendFraction;

                    double b2Duration =
                        CanningTownBlendCycleSeconds -
                        b3Duration;

                    if (phase < b2Duration)
                    {
                        return 2;
                    }

                    return 3;
                }

                /*
                 * Generic adaptive braking for stations
                 * added later.
                 */
                double distanceToStopMetres =
                    station.StopPositionMetres -
                    locationMetres;

                double speedMps =
                    Math.Max(
                        0.0,
                        speedKph) / 3.6;

                double usableDistanceMetres =
                    Math.Max(
                        1.0,
                        distanceToStopMetres);

                double requiredDecelerationMps2 =
                    speedMps * speedMps /
                    (2.0 * usableDistanceMetres);

                if (requiredDecelerationMps2 >=
                    StationB3ThresholdMps2)
                {
                    return 3;
                }

                if (requiredDecelerationMps2 >=
                    StationB2ThresholdMps2)
                {
                    return 2;
                }

                return 1;
            }

            return 0;
        }

        public static int GetGreenParkSpeedLimitBrakeNotch(
    double locationMetres,
    double speedKph)
        {
            /*
             * Only supervise the final approach to
             * the Green Park 35 mph limit.
             */
            if (locationMetres < 16400.0 ||
                locationMetres > GreenPark35LimitMetres)
            {
                greenParkSpeedLimitB3Active = false;
                return 0;
            }

            /*
             * Once B3 begins, latch it on.
             * Do not recalculate every frame and cause
             * B3 / B0 cycling.
             */
            if (greenParkSpeedLimitB3Active)
            {
                if (speedKph <=
                    GreenPark35Kph +
                    GreenParkBrakeReleaseToleranceKph)
                {
                    greenParkSpeedLimitB3Active = false;
                    return 0;
                }

                return 3;
            }

            double distanceToLimitMetres =
                GreenPark35LimitMetres -
                locationMetres;

            if (distanceToLimitMetres <= 0.0)
            {
                return 0;
            }

            if (speedKph <= GreenPark35Kph)
            {
                return 0;
            }

            double speedMps =
                speedKph / 3.6;

            double targetMps =
                GreenPark35Kph / 3.6;

            double requiredDecelerationMps2 =
                ((speedMps * speedMps) -
                 (targetMps * targetMps)) /
                (2.0 * distanceToLimitMetres);

            /*
             * Let drag and the steep uphill gradient
             * slow the train naturally first.
             *
             * B3 only begins once more substantial
             * braking is actually required.
             */
            if (requiredDecelerationMps2 >=
                GreenParkBrakeTriggerDecelerationMps2)
            {
                greenParkSpeedLimitB3Active = true;
                return 3;
            }

            return 0;
        }

        public static int GetSpeedLimitBrakeNotch(
            double locationMetres,
            double speedKph)
        {
            if (locationMetres < DeepLevelStartMetres)
            {
                return 0;
            }

            double currentLimitKph =
                JubileeRouteProfile.GetCurrentSpeedLimitKph(
                    locationMetres);

            SpeedLimitPoint nextLowerLimit =
                GetNextLowerSpeedLimit(
                    locationMetres,
                    currentLimitKph);

            if (nextLowerLimit == null)
            {
                return 0;
            }

            bool isLondonBridge40Limit =
    Math.Abs(
        nextLowerLimit.PositionMetres -
        LondonBridge40LimitMetres) < 0.01;

            if (isLondonBridge40Limit)
            {
                return 0;
            }

            bool isGreenPark35Limit =
    Math.Abs(
        nextLowerLimit.PositionMetres -
        GreenPark35LimitMetres) < 0.01;

            if (isGreenPark35Limit)
            {
                return 0;
            }

            double distanceToLimitMetres =
                nextLowerLimit.PositionMetres -
                locationMetres;

            if (distanceToLimitMetres <= 0.0)
            {
                return 0;
            }

            bool isSwissCottage40Limit =
    Math.Abs(
        nextLowerLimit.PositionMetres -
        SwissCottage40LimitMetres) < 0.01;

            if (isSwissCottage40Limit)
            {
                double swissCottageDistanceToLimitMetres =
                    SwissCottage40LimitMetres -
                    locationMetres;

                /*
                 * Once sufficiently close to the target speed,
                 * release the speed-limit brake.
                 */
                if (speedKph <=
                    SwissCottage40Kph +
                    SwissCottage40ReleaseToleranceKph)
                {
                    return 0;
                }

                /*
                 * Final section: B3.
                 */
                if (swissCottageDistanceToLimitMetres <=
                    SwissCottage40B3StartDistanceMetres)
                {
                    return 3;
                }

                /*
                 * Initial gentler reduction: B2.
                 */
                if (swissCottageDistanceToLimitMetres <=
                    SwissCottage40B2StartDistanceMetres)
                {
                    return 2;
                }

                return 0;
            }

            bool isFinchleyRoad38Limit =
    Math.Abs(
        nextLowerLimit.PositionMetres -
        FinchleyRoad38LimitMetres) < 0.01;

            if (isFinchleyRoad38Limit)
            {
                double finchleyRoadDistanceToLimitMetres =
                    FinchleyRoad38LimitMetres -
                    locationMetres;

                if (speedKph <=
                    FinchleyRoad38Kph +
                    FinchleyRoad38ReleaseToleranceKph)
                {
                    return 0;
                }

                /*
                 * Surface-style speed reduction:
                 * B2 only. Never B3.
                 */
                if (finchleyRoadDistanceToLimitMetres <=
                    FinchleyRoad38B2StartDistanceMetres)
                {
                    return 2;
                }

                return 0;
            }

            double b3BrakingDistanceMetres =
                CalculateB3BrakingDistanceMetres(
                    currentLimitKph,
                    nextLowerLimit.SpeedKph);

            b3BrakingDistanceMetres +=
                DeepLevelB3StartMarginMetres;

            /*
             * Haven't reached the calculated B3
             * braking point yet.
             */
            if (distanceToLimitMetres >
                b3BrakingDistanceMetres)
            {
                return 0;
            }

            /*
             * Once B3 starts, hold it continuously
             * until the new speed has been reached.
             */
            if (speedKph >
                nextLowerLimit.SpeedKph +
                DeepLevelB3ReleaseToleranceKph)
            {
                return 3;
            }

            return 0;
        }

        private static int GetMarkerCaptureBrakeNotch(
            double stopPositionMetres,
            double locationMetres,
            double speedKph,
            double captureDistanceMetres)
        {
            double remainingMetres =
                stopPositionMetres -
                locationMetres;

            if (remainingMetres >
                captureDistanceMetres)
            {
                return -1;
            }

            if (remainingMetres <= 0.0)
            {
                if (speedKph > 0.5)
                {
                    return 3;
                }

                return 2;
            }

            double captureSpeedMps =
                speedKph / 3.6;

            double captureRequiredDecelerationMps2 =
                (captureSpeedMps * captureSpeedMps) /
                (2.0 * remainingMetres);

            if (captureRequiredDecelerationMps2 >=
                MarkerCaptureB3ThresholdMps2)
            {
                return 3;
            }

            if (captureRequiredDecelerationMps2 >=
                MarkerCaptureB2ThresholdMps2)
            {
                return 2;
            }

            return 1;
        }

        private static double CalculateApproachSpeedKph(
            double targetSpeedKph,
            double distanceMetres)
        {
            if (distanceMetres <= 0.0)
            {
                return targetSpeedKph;
            }

            double targetSpeedMps =
                targetSpeedKph / 3.6;

            /*
             * Calculate the maximum present speed that
             * allows the train to reach targetSpeedKph
             * over the remaining distance using the
             * assumed approach deceleration.
             */
            double permittedSpeedMps =
                Math.Sqrt(
                    targetSpeedMps * targetSpeedMps +
                    2.0 *
                    ApproachDecelerationMps2 *
                    distanceMetres);

            return permittedSpeedMps * 3.6;
        }
    }
}