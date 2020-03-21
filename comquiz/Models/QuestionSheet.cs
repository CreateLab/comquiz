﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace comquiz
{
    public class QuestionSheet
    {
        public string QuestionBody { get; set; } = "NO-BODY";

        public List<AnswerSheet> OriginalAnswersList { get; set; } = new List<AnswerSheet>();

        public List<AnswerSheet> PersonalizedAnswersList
        {
            get
            {
                List<AnswerSheet> answersList = new List<AnswerSheet>(OriginalAnswersList);

                if (Preferences != null)
                {
                    if (Preferences.RandomizeAnswersOrder)
                    {
                        answersList.Shuffle<AnswerSheet>().ToList<AnswerSheet>();
                    }

                }

                return answersList;
            }

        }


        public int NumberOfRightAnswers
        {
            get
            {
                int rightAnswers = 0;

                foreach (AnswerSheet singleAnswer in PersonalizedAnswersList)
                {
                    if (singleAnswer.IsTheRightAnswer) rightAnswers++;
                }

                return rightAnswers;

            }
        }

        QuizPreferences _preferences;
        public QuizPreferences Preferences
        {
            get => _preferences;

            set
            {
                _preferences = value;

                foreach (AnswerSheet singleAnswer in PersonalizedAnswersList)
                {
                    singleAnswer.Preferences = value;
                }
            }
        }

        public bool ShowRightAnswers { get; set; } = false;





        public QuestionSheet()
        {

        }

        public ANSWERED GetQuestionStatus()
        {

            short answersDone = 0;

            // Check: I answered?

            foreach (AnswerSheet singleAnswer in PersonalizedAnswersList)
            {
                if (singleAnswer.HasBeenSelected)
                {
                    answersDone++;
                }
            }

            if (answersDone != NumberOfRightAnswers)
            {
                return ANSWERED.NotYet;
            }

            else
            {
                // I've answered right?

                foreach (AnswerSheet singleAnswer in PersonalizedAnswersList)
                {
                    if (singleAnswer.IsTheRightAnswer && !singleAnswer.HasBeenSelected)
                    {
                        return ANSWERED.Wrong;
                    }

                    if (!singleAnswer.IsTheRightAnswer && singleAnswer.HasBeenSelected)
                    {
                        return ANSWERED.Wrong;
                    }

                }

                // All traps avoided

                return ANSWERED.Correctly;

            }

        }

        public static List<AnswerSheet> RemoveListAnnotationsIfPossible(List<AnswerSheet> inputList)
        {
            bool answersAreTrimmable = true;

            foreach (AnswerSheet singleAnswer in inputList)
            {
                if (!Regex.IsMatch(singleAnswer.AnswerBody, @"^([A-z]|[0-9]){1}(\)|\.)[\s\S\.*]*"))
                {
                    answersAreTrimmable = false;
                    break;
                }
            }

            if (answersAreTrimmable)
            {
                foreach (AnswerSheet singleAnswer in inputList)
                {
                    singleAnswer.AnswerBody = Regex.Replace(singleAnswer.AnswerBody, @"^([A-z]|[0-9]){1}(\)|\.)[\s\S\.*]*", "");
                }

            }

            return inputList;

        }

        public enum ANSWERED
        {
            NotYet,
            Correctly,
            Wrong
        }

    }

}
