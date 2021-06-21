using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blake3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Surveys.Model;

namespace Surveys.Controllers
{
    public class SurveysController : Controller
    {
        private readonly surveyContext _context;

        public SurveysController(surveyContext context)
        {
            _context = context;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            var surveyContext = _context.Surveys.Include(s => s.IdUserNavigation);
            return View(await surveyContext.ToListAsync());
        }
        // GET: Surveys
        [Authorize]//(Roles = "Admin,User,Owner")
        public async Task<IActionResult> IndexUncompletedSurveys()
        {
            var surveyContext = _context.Surveys;//.Where(s => s.IdUserNavigation.Id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View(await surveyContext.ToListAsync());
        }
        // GET: Surveys
        [Authorize]//(Roles = "Admin,User,Owner")
        public async Task<IActionResult> IndexCreatedSurveys()
        {
            var surveyContext = _context.Surveys.Where(s => s.IdUserNavigation.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View(await surveyContext.ToListAsync());
        }
        public IActionResult Success(string hash)
        {
            ViewData["computedHash"] = hash;
            return View();
        }
        public IActionResult Export(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var counters = new Dictionary<string, Dictionary<string, int>>();
            var id_user = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var eligibleUsers = _context.Users.Count();//.Where(u => u.Id != id_user)
            var surveysCompleted = _context.SubmittedSurveys.Where(i => i.IdSurvey == id.Value).Count();
            var survey = _context.Surveys.Find(id);
            if (survey == null)
            {
                return NotFound();
            }
            counters.Add("SurveyData", new Dictionary<string, int>());
            //counters["SurveyData"][nameof(eligibleUsers)] = eligibleUsers;
            counters["SurveyData"].Add(nameof(surveysCompleted), surveysCompleted);
            var contents = _context.SurveysContents
                .Where((s) => s.IdSurvey == id.Value)
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                .ToList();
            var answers = _context.SubmittedSurveyAnswers
                 .Include(s => s.IdSubmittedSurveyNavigation)
                 .ThenInclude(s => s.IdSurveyNavigation)
                 .Where(s => s.IdSubmittedSurveyNavigation.IdSurveyNavigation.Id == id.Value)
                 .Include(s => s.IdQuestionNavigation)
                 .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                 .ToList();

            foreach (var answer in answers)
            {
                if (counters.ContainsKey(answer.IdQuestionNavigation.Question1) is false)
                    counters.Add(answer.IdQuestionNavigation.Question1, new Dictionary<string, int>());
                var currItem = contents.First(i => i.IdQuestion == answer.IdQuestion);
                if (currItem.IdQuestionNavigation.PredefinedAnswers.Count > 1)
                {
                    var k = 0;
                    var arr = answer.Answers.Split(',');
                    foreach (var a in currItem.IdQuestionNavigation.PredefinedAnswers)
                    {
                        if (counters[answer.IdQuestionNavigation.Question1].ContainsKey(a.Answer) is false)
                            counters[answer.IdQuestionNavigation.Question1].Add(a.Answer, 0);
                        if (currItem.AllowMultipleAnswers && arr[k] is "true")
                            counters[answer.IdQuestionNavigation.Question1][a.Answer] += 1;
                        else if (arr[0] == a.Answer)
                            counters[answer.IdQuestionNavigation.Question1][a.Answer] += 1;
                        k++;
                    }
                }
                else
                {
                    if (counters[answer.IdQuestionNavigation.Question1].ContainsKey("pytanie otwarte") is false)
                        counters[answer.IdQuestionNavigation.Question1].Add("pytanie otwarte", 0);
                    if (string.IsNullOrEmpty(answer.Answers) is false)
                        counters[answer.IdQuestionNavigation.Question1]["pytanie otwarte"] += 1;
                }
            }
            byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(counters);
            return File(jsonString, "application/json", $"{survey.Topic}.json");
        }
        // GET: Surveys/Statistics/5
        public async Task<IActionResult> Statistics(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contents = _context.SurveysContents
                .Where((s) => s.IdSurvey == id.Value)
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                .ToList();
            var answers = _context.SubmittedSurveyAnswers
                 .Include(s => s.IdSubmittedSurveyNavigation)
                 .ThenInclude(s => s.IdSurveyNavigation)
                 .Where(s => s.IdSubmittedSurveyNavigation.IdSurveyNavigation.Id == id.Value)
                 .Include(s => s.IdQuestionNavigation)
                 .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                 .ToList();
            var counters = new Dictionary<int, Dictionary<int, int>>();
            foreach (var answer in answers)
            {
                if (counters.ContainsKey(answer.IdQuestion) is false)
                    counters.Add(answer.IdQuestion, new Dictionary<int, int>());
                var currItem = contents.First(i => i.IdQuestion == answer.IdQuestion);
                if (currItem.IdQuestionNavigation.PredefinedAnswers.Count > 1)
                {
                    var k = 0;
                    var arr = answer.Answers.Split(',');
                    foreach (var a in currItem.IdQuestionNavigation.PredefinedAnswers)
                    {
                        if (counters[answer.IdQuestion].ContainsKey(k) is false)
                            counters[answer.IdQuestion].Add(k, 0);
                        if (currItem.AllowMultipleAnswers && arr[k] is "true")
                            counters[answer.IdQuestion][k] += 1;
                        else if (arr[0] == a.Answer)
                            counters[answer.IdQuestion][k] += 1;
                        k++;
                    }
                }
                else
                {
                    if (counters[answer.IdQuestion].ContainsKey(0) is false)
                        counters[answer.IdQuestion].Add(0, 0);
                    if (string.IsNullOrEmpty(answer.Answers) is false)
                        counters[answer.IdQuestion][0] += 1;
                }
            }
            ViewData[nameof(contents)] = contents;
            ViewData[nameof(counters)] = counters;
            var id_user = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["eligibleUsers"] = _context.Users.Count();//.Where(u => u.Id != id_user)
            ViewData["surveysCompleted"] = _context.SubmittedSurveys.Where(i => i.IdSurvey == id.Value).Count();
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }
        // GET: Surveys/Complete/5
        public async Task<IActionResult> Complete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["contents"] = _context.SurveysContents
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                .Where((s) => s.IdSurvey == id.Value).ToList();
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Complete/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int? id, string[][] answer)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contents = _context.SurveysContents
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Include(s => s.IdQuestionNavigation.PredefinedAnswers)
                .Where((s) => s.IdSurvey == id.Value).ToList();
            ViewData["contents"] = contents;
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            var i = 0;
            foreach (var question in ViewData["contents"] as List<SurveysContent>)
            {
                bool answered = false;
                if (question.Required)
                {
                    foreach (var a in answer[i])
                        if (a is not null and not "notSelected" and not "false")
                            answered = true;
                }
                else answered = true;
                if (answered is false)
                    ModelState.AddModelError($"answer[{i}]", "Musisz odpowiedzieć na to pytanie");
                i++;
            }
            if (ModelState.IsValid)
            {
                string stringHash = null;
                try
                {
                    var submittedSurvey = new SubmittedSurvey() { IdSurveyNavigation = survey };
                    List<SubmittedSurveyAnswer> answers = new();
                    var j = 0;
                    using var hasher = Hasher.New();
                    var id_user = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = _context.Users.Find(id_user);
                    byte[] idBytes = BitConverter.GetBytes(user.Id);
                    byte[] bytes = BitConverter.GetBytes(submittedSurvey.IdSurvey);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(idBytes);
                        Array.Reverse(bytes);
                    }
                    hasher.Update(idBytes);
                    hasher.Update(bytes);

                    foreach (var content in contents)
                    {
                        var a = new SubmittedSurveyAnswer();
                        a.IdSubmittedSurveyNavigation = submittedSurvey;
                        a.IdQuestionNavigation = content.IdQuestionNavigation;
                        a.Answers = string.Join(',', answer[j]);
                        answers.Add(a);
                        hasher.Update(Encoding.UTF8.GetBytes(a.IdQuestionNavigation.Question1));
                        hasher.Update(Encoding.UTF8.GetBytes(a.Answers));
                        j++;
                    }
                    var hash = hasher.Finalize();
                    stringHash = hash.ToString();
                    foreach (var addHash in answers)
                    {
                        addHash.Hash = stringHash;
                    }
                    _context.SubmittedSurveys.Add(submittedSurvey);
                    _context.SubmittedSurveyAnswers.AddRange(answers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /* if (!SurveyExists(survey.Id))
                     {
                         return NotFound();
                     }
                     else
                     {
                         throw;
                     }*/
                }
                return RedirectToAction(nameof(Success), new { hash = stringHash });
            }

            return View(survey);
        }
        // GET: Surveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .Include(s => s.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            ViewData["contents"] = contents;
            ViewData["existingContents"] = _context.SurveysContents
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Where((s) => s.IdSurveyNavigation.IdUser == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View();
        }

        private static List<SurveysContent> contents = new();

        // POST: Surveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Topic,MinBirthYear,MaxBirthYear,Sex")] Survey survey, string @new, string create, string create_answer, string[] q, bool[][] config, string[][] a, string delete_question, string delete_answer, string add_existing, string existingQ)
        {
            var j = 0;
            foreach (var content in contents)
            {
                content.IdSurveyNavigation = survey;
                content.IdQuestionNavigation.Question1 = q[j];
                content.AllowMultipleAnswers = config[j][1];
                content.Required = config[j][0];
                var id_a = 0;
                foreach (var answer in content.IdQuestionNavigation.PredefinedAnswers)
                {
                    answer.Answer = a[j][id_a];
                    id_a++;
                }
                j++;
            }
            ViewData["contents"] = contents;
            ViewData["existingContents"] = _context.SurveysContents
                .Include(s => s.IdSurveyNavigation)
                .Include(s => s.IdQuestionNavigation)
                .Where((s) => s.IdSurveyNavigation.IdUser == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (string.IsNullOrEmpty(create) is false)
            {
                if (string.IsNullOrEmpty(survey.Topic))
                    ModelState.AddModelError(nameof(survey.Topic), "Temat nie może być pusty");
                if (survey.MinBirthYear > survey.MaxBirthYear)
                    ModelState.AddModelError(nameof(survey.MinBirthYear), "Reguła musi być mniejsza niż maksymalny rok");
                var i = 0;
                foreach (var question in q)
                {
                    if (string.IsNullOrEmpty(question))
                        ModelState.AddModelError($"q[{i}]", "Pytanie nie może być puste");
                    i++;
                }
                /*i = 0;
                foreach (var answers in a)
                {
                    foreach (var answer in answers)
                    {
                        if (answers.Length > 0 && string.IsNullOrEmpty(answer))
                            ModelState.AddModelError($"q[{i}]", "Przy wielokrotnych odpowiedziach, żadna odpowiedź nie może być pusta");
                        else if (answers.Length == 0 && )
                    }
                    i++;
                }*/
            }
            else if (string.IsNullOrEmpty(delete_question) is false)
            {
                var id = int.Parse(delete_question);
                var tobeRemoved = contents[id];
                contents.RemoveAt(id);
            }
            else if (string.IsNullOrEmpty(delete_answer) is false)
            {
                var splits = delete_answer.Split(',');
                var id_q = int.Parse(splits[0]);
                var id_a = int.Parse(splits[1]);
                var toBeRemoved = contents[id_q].IdQuestionNavigation.PredefinedAnswers.ElementAt(id_a);
                contents[id_q].IdQuestionNavigation.PredefinedAnswers.Remove(toBeRemoved);
            }
            else if (string.IsNullOrEmpty(add_existing) is false)
            {
                var id = int.Parse(existingQ);
                var question = _context.Questions.Where(i => i.Id == id).Include(s => s.PredefinedAnswers).First();
                contents.Add(new SurveysContent() { IdQuestionNavigation = question });
            }
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(create_answer) is false)
                {
                    var id = int.Parse(create_answer.Split(':')[1]);
                    contents[id].IdQuestionNavigation.PredefinedAnswers.Add(new PredefinedAnswer());
                }
                else if (string.IsNullOrEmpty(@new) is false)
                {
                    AddNewQuestion(survey);
                }
                else if (string.IsNullOrEmpty(create) is false)
                {
                    survey.IdUserNavigation = _context.Users.Find(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

                    _context.Add(survey);

                    _context.AddRange(contents);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(survey); ;
        }
        private void AddNewQuestion(Survey s)
        {
            var question = new Question();
            question.PredefinedAnswers.Add(new PredefinedAnswer());
            contents.Add(new SurveysContent() { IdQuestionNavigation = question });

        }
        // GET: Surveys/Audit
        public IActionResult Audit()
        {
            return View();
        }

        // POST: Surveys/Audit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Audit(string audit, string hash)
        {

            if (string.IsNullOrEmpty(audit) is false && string.IsNullOrEmpty(hash) is false)
            {
                var qAndAs = _context.SubmittedSurveyAnswers.Where(i => i.Hash == hash).Include(i => i.IdQuestionNavigation).Include(i => i.IdSubmittedSurveyNavigation);
                if (qAndAs is null || qAndAs.Any() is false)
                {
                    ModelState.AddModelError(nameof(hash), "Hash nie zwrócił żadnej ankiety");
                    return View();
                }
                var submittedSurvey = qAndAs.First().IdSubmittedSurveyNavigation;
                using var hasher = Hasher.New();
                var id_user = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = _context.Users.Find(id_user);
                byte[] idBytes = BitConverter.GetBytes(user.Id);
                byte[] bytes = BitConverter.GetBytes(submittedSurvey.IdSurvey);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(idBytes);
                    Array.Reverse(bytes);
                }
                hasher.Update(idBytes);
                hasher.Update(bytes);
                foreach (var content in qAndAs)
                {
                    hasher.Update(Encoding.UTF8.GetBytes(content.IdQuestionNavigation.Question1));
                    hasher.Update(Encoding.UTF8.GetBytes(content.Answers));
                }
                var finalHash = hasher.Finalize();
                var stringHash = finalHash.ToString();

                if (stringHash != hash)
                    ViewData["changesDetected"] = true;
                else
                    ViewData["changesDetected"] = false;
            }
            else
                ModelState.AddModelError(nameof(hash), "Hash nie może być pusty");
            return View();
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}
