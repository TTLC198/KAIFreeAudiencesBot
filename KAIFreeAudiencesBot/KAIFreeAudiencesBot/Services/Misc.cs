﻿using KAIFreeAudiencesBot.Models;
using System.Globalization;
using KAIFreeAudiencesBot.Services.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace KAIFreeAudiencesBot.Services;

public static class Misc
{
    public static Parity GetWeekParity(DateTime? time)
    {
        var myCI = new CultureInfo("ru-RU");
        var myCal = myCI.Calendar;
        return myCal.GetWeekOfYear(time ??= DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) % 2 == 0
            ? Parity.Even
            : Parity.NotEven;
    }

    public static DateTime? GetCurrentDay(DateTime? time, Parity parity)
    {
        time ??= DateTime.Now;
        for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1; i++)
        {
            var tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);
            if (tempTime.DayOfWeek == time.Value.DayOfWeek && GetWeekParity(tempTime) == parity) return tempTime;
        }

        return null;
    }

    private static long GetTime(this DateTime dateTime)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)((dateTime - epoch).TotalMilliseconds);
    }

    public static int[] GetIndexes(InlineKeyboardMarkup keyboardMarkup, string match)
    {
        var i = 0;
        var j = 0;
        foreach (var arrayOfButtons in keyboardMarkup!.InlineKeyboard.ToList())
        {
            foreach (var button in arrayOfButtons.ToList())
            {
                if (button.CallbackData == match)
                {
                    return new[] { j, i };
                }

                i++;
            }

            i = 0;
            j++;
        }

        return new[] { -1, -1 };
    }

    public static void ChangeValue(ClientSteps clientStep, ClientSettings clientSettings, InlineKeyboardButton button)
    {
        switch (clientStep)
        {
            case ClientSteps.Default:
                break;
            case ClientSteps.ChooseParity:
                switch (button.CallbackData!.Split('_')[1])
                {
                    case "e":
                        if (clientSettings.Parity.IndexOf(Parity.Even) == -1)
                        {
                            clientSettings.Parity.Add(Parity.Even);
                        }
                        else
                        {
                            clientSettings.Parity.Remove(Parity.Even);
                        }

                        break;
                    case "n":
                        if (clientSettings.Parity.IndexOf(Parity.NotEven) == -1)
                        {
                            clientSettings.Parity.Add(Parity.NotEven);
                        }
                        else
                        {
                            clientSettings.Parity.Remove(Parity.NotEven);
                        }

                        break;
                }

                break;
            case ClientSteps.ChooseDay:
                var dayOfWeek =
                    Enum.GetValues(typeof(DaysOfWeek)).Cast<DaysOfWeek>().ToList()[
                        int.Parse(button.CallbackData!.Split('_')[1])];
                if (clientSettings.DaysOfWeek.IndexOf(dayOfWeek) == -1)
                {
                    clientSettings.DaysOfWeek.Add(dayOfWeek);
                }
                else
                {
                    clientSettings.DaysOfWeek.Remove(dayOfWeek);
                }
                break;
            case ClientSteps.ChooseBuilding:
                break;
            case ClientSteps.ChooseTime:
                break;
            case ClientSteps.ChooseCorrectTime:
                break;
            case ClientSteps.ChooseAudience:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(clientStep), clientStep, null);
        }
    }

    public static InlineKeyboardMarkup UpdateKeyboardMarkup(ClientSteps clientStep, ClientSettings clientSettings,
        InlineKeyboardMarkup keyboard)
    {
        switch (clientStep)
        {
            case ClientSteps.Default:
                break;
            case ClientSteps.ChooseParity:
                keyboard.InlineKeyboard.ToArray()[0].ToArray()[0].Text =
                    clientSettings.Parity.IndexOf(Parity.Even) != -1
                        ? keyboard.InlineKeyboard.ToArray()[0].ToArray()[0].Text.Replace("☑", "✅")
                        : keyboard.InlineKeyboard.ToArray()[0].ToArray()[0].Text.Replace("✅", "☑");
                keyboard.InlineKeyboard.ToArray()[0].ToArray()[1].Text =
                    clientSettings.Parity.IndexOf(Parity.NotEven) != -1
                        ? keyboard.InlineKeyboard.ToArray()[0].ToArray()[1].Text.Replace("☑", "✅")
                        : keyboard.InlineKeyboard.ToArray()[0].ToArray()[1].Text.Replace("✅", "☑");
                break;
            case ClientSteps.ChooseDay:
                var days = Enum.GetValues(typeof(DaysOfWeek)).Cast<DaysOfWeek>().ToList();
                var buttons = keyboard.InlineKeyboard!.ToArray();
                for (var i = 0; i < buttons.Length - 1; i++)
                {
                    for (var j = 0; j < buttons[i].ToArray().Length; j++)
                    {
                        keyboard.InlineKeyboard.ToArray()[i].ToArray()[j].Text =
                            clientSettings.DaysOfWeek.IndexOf(days[i*3+j]) != -1
                                ? keyboard.InlineKeyboard.ToArray()[i].ToArray()[j].Text.Replace("☑", "✅")
                                : keyboard.InlineKeyboard.ToArray()[i].ToArray()[j].Text.Replace("✅", "☑");
                    }
                }
                break;
            case ClientSteps.ChooseBuilding:
                break;
            case ClientSteps.ChooseTime:
                break;
            case ClientSteps.ChooseCorrectTime:
                break;
            case ClientSteps.ChooseAudience:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(clientStep), clientStep, null);
        }

        return keyboard;
    }
}