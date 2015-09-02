using HistoryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HistoryWebsite.Controllers
{
    public class QuizController : Controller
    {
        private louisemc_tudorsEntities _entities = new louisemc_tudorsEntities();
       
        // GET: Quiz
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Quiz(int quizId)
        {
            Quizze quizToDisplay = _entities.Quizzes.Find(quizId);
           
            return View(quizToDisplay);
        }

        [HttpPost]
        public ActionResult Quiz(FormCollection form)
        {
            int formLength = form.Count;
            string quizName = (string)form[formLength - 1];
            int quizLength = formLength - 1;
            int[] quizAnswers = new int[quizLength];
            int quizId = 0;

            //get quiz answers from form and put them into an array
            for (int i = 0; i < quizLength; i++)
            {
                quizAnswers[i] = Convert.ToInt32(form[i]);
            }
            TempData["quizAnswers"] = quizAnswers;
            TempData["quizLength"] = quizLength;

            //call appropriate quiz scoring method
            if(quizName == "Submit Houses Quiz")
            {
                ScoreHousesQuiz();
                int resultId = (int)TempData["resultId"];
                //send quiz id, so that results feedback can be adjusted
                quizId = 1;
                TempData["quizId"] = quizId;
                return RedirectToAction(actionName: "Result", routeValues: new { id = resultId });
            }
            else
            {
                ScoreCharactersQuiz();
                int resultId = (int)TempData["resultId"];
                //send quiz id, so that results feedback can be adjusted
                quizId = 2;
                TempData["quizId"] = quizId;
                return RedirectToAction(actionName: "Result", routeValues: new { id = resultId });
            }
        }

        public void ScoreHousesQuiz()
        {
           int quizLength = (int)TempData["quizLength"];
           int[] quizAnswers = new int[quizLength];
           int quizScore = 0;

           quizAnswers = (int[])TempData["quizAnswers"];

           // check quiz answers and work out the score
           for (int i = 0; i < quizLength; i++)
           {
               int currentQuizAnswer = quizAnswers[i];

               //check answer against DB
               var checkedAnswer = (from answer in _entities.Answers
                                    where answer.Id == currentQuizAnswer
                                   select answer.Correct).Single();

               bool answerCorrect = Convert.ToBoolean(checkedAnswer);

               if (answerCorrect)
               {
                   quizScore++;
               }
           }

           int resultId = 0;

           //determine which results content will be retrieved
           if (quizScore < 4)
           {
               resultId = 1002;
           }
           else if (quizScore < 8)
           {
               resultId = 2;
           }
           else
           {
               resultId = 1;
           }

           TempData["resultId"] = resultId;
           TempData["quizScore"] = quizScore;
        }

               
         public void ScoreCharactersQuiz()
        {
            int numCategories = 8;
            int[,] quizCategoryScores = new int[numCategories, 2];
            int highestScore = 0;
            int resultId = 0;

            //put category labels into scoring array
            for (int i = 0; i < numCategories; i++)
            {
                quizCategoryScores[i, 0] = i + 1;
            }

            //get the number of answers
           int quizLength = (int)TempData["quizLength"];
           int[] quizAnswers = new int[quizLength];

           //get the answers
           quizAnswers = (int[])TempData["quizAnswers"];

           

           // check quiz answers and work out the score
           for (int i = 0; i < quizLength; i++)
           {
               int currentQuizAnswer = quizAnswers[i];

               //get catgory of each answer from DB
                var checkedAnswer = (from answer in _entities.Answers
                                     where answer.Id == currentQuizAnswer
                                     select answer.Category).Single();

                //tally the score for each category
                switch(checkedAnswer)
                {
                    case 1:
                        quizCategoryScores[0, 1]++;
                        break;
                    case 2:
                        quizCategoryScores[1, 1]++;
                        break;
                    case 3:
                        quizCategoryScores[2, 1]++;
                        break;
                    case 4:
                        quizCategoryScores[3, 1]++;
                        break;
                    case 5:
                        quizCategoryScores[4, 1]++;
                        break;
                    case 6:
                        quizCategoryScores[5, 1]++;
                        break;
                    case 7:
                        quizCategoryScores[6, 1]++;
                        break;
                    case 8:
                        quizCategoryScores[7, 1]++;
                        break;
                    default:
                        break;
                }//switch
            }//for

            //find the highest score - doesn't do tiebreaks at the moment
            for (int i = 0; i < numCategories; i++)
            {
                if(quizCategoryScores[i,1] > highestScore)
                {
                    highestScore = quizCategoryScores[i,0];
                }
            }

             //determine which results content will be retrieved
            switch(highestScore)
            {
                case 1:
                    resultId = 1003;
                    break;
                case 2:
                    resultId = 2002;
                    break;
                case 3:
                    resultId = 2003;
                    break;
                case 4:
                    resultId = 2004;
                    break;
                case 5:
                    resultId = 2005;
                    break;
                case 6:
                    resultId = 2007;
                    break;
                case 7:
                    resultId = 2009;
                    break;
                case 8:
                    resultId = 2010;
                    break;
                default:
                    resultId = 1003;
                    break;
            }
            TempData["resultId"] = resultId;
        }

        public PartialViewResult _Questions(int quizId)
        {
            ViewBag.QuizId = quizId;
           
            var questionList = from question in _entities.Questions
                               where question.QuizId == quizId
                               select question;

            return PartialView(questionList);
        }

        public PartialViewResult _Answers(int questionId)
        {            
            var answerList = from answer in _entities.Answers
                               where answer.QuestionId == questionId
                               select answer;

            return PartialView(answerList);
        }

        public ActionResult GetImage(int id)
        {
            var firstOrDefault = _entities.Images.Where(i => i.Id == id).FirstOrDefault();
            if (firstOrDefault != null)
            {
                byte[] image = firstOrDefault.Image1;
                return File(image, "image/jpg");
            }
            else
            {
                return null;
            }

        }

        public ActionResult Result()
        {
            int resultId = (int)TempData["resultId"];
            int quizId = (int)TempData["quizId"];
            //display score only for Houses Quiz
            if (quizId == 1)
            { 
                int quizScore = (int)TempData["quizScore"];
                ViewBag.QuizScore = quizScore;
            }
            var correctResult = (from result in _entities.Results
                             where result.Id == resultId
                             select result).Single();

            return View(correctResult);
        }

    }
}