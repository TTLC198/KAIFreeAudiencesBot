﻿namespace KAIFreeAudiencesBot.Models;

public enum Parity
{
    Even,
    NotEven
}

public enum Modes
{
    Default,
    SpecificDates,
    SpecificDaysOfWeek,
    SpecificDaysAllAudiences,
}

public enum Buildings
{
    All,
    First,
    Second, 
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth
}

public enum ClientSteps
{
    Default,
    ChooseParity,
    ChooseBuilding,
    ChooseBuildingAllAudiences,
    ChooseDay,
    ChooseTime,
    ChooseCorrectTime,
    ChooseAudience,
}