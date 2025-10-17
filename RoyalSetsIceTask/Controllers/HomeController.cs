using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoyalSetsIceTask.Models;
using System.Collections.Generic;

namespace RoyalSetsIceTask.Controllers
{
    public class HomeController : Controller
    {
        private static FamilyNode Root = BuildInitialTree();

        private static FamilyNode BuildInitialTree()
        {
            var root = new FamilyNode(new RoyalMember("King Charles III", new DateTime(1948, 11, 14)));

            var william = root.AddChild(new RoyalMember("William, Prince of Wales", new DateTime(1982, 6, 21)));
            var harry = root.AddChild(new RoyalMember("Harry, Duke of Sussex", new DateTime(1984, 9, 15)));

            william.AddChild(new RoyalMember("Prince George of Wales", new DateTime(2013, 7, 22)));
            william.AddChild(new RoyalMember("Princess Charlotte of Wales", new DateTime(2015, 5, 2)));
            william.AddChild(new RoyalMember("Prince Louis of Wales", new DateTime(2018, 4, 23)));

            harry.AddChild(new RoyalMember("Prince Archie of Sussex", new DateTime(2019, 5, 6)));
            harry.AddChild(new RoyalMember("Princess Lilibet of Sussex", new DateTime(2021, 6, 4)));

            var anne = new FamilyNode(new RoyalMember("Anne, Princess Royal", new DateTime(1950, 8, 15)));
            root.Children.Add(anne); anne.Parent = root;

            var peter = anne.AddChild(new RoyalMember("Peter Phillips", new DateTime(1977, 11, 15)));
            peter.AddChild(new RoyalMember("Savannah Phillips", new DateTime(2010, 12, 29)));
            peter.AddChild(new RoyalMember("Isla Phillips", new DateTime(2012, 3, 29)));

            var zara = anne.AddChild(new RoyalMember("Zara Tindall", new DateTime(1981, 5, 15)));
            zara.AddChild(new RoyalMember("Mia Tindall", new DateTime(2014, 1, 17)));
            zara.AddChild(new RoyalMember("Lena Tindall", new DateTime(2018, 6, 18)));
            zara.AddChild(new RoyalMember("Lucas Tindall", new DateTime(2021, 3, 21)));

            var andrew = root.AddChild(new RoyalMember("Andrew, Duke of York", new DateTime(1960, 2, 19)));
            var beatrice = andrew.AddChild(new RoyalMember("Princess Beatrice", new DateTime(1988, 8, 8)));
            beatrice.AddChild(new RoyalMember("Sienna Mapelli Mozzi", new DateTime(2021, 9, 18)));
            var eugenie = andrew.AddChild(new RoyalMember("Princess Eugenie", new DateTime(1990, 3, 23)));
            eugenie.AddChild(new RoyalMember("August Brooksbank", new DateTime(2021, 2, 9)));

            var edward = root.AddChild(new RoyalMember("Edward, Duke of Edinburgh", new DateTime(1964, 3, 10)));
            edward.AddChild(new RoyalMember("Lady Louise Windsor", new DateTime(2003, 11, 8)));
            edward.AddChild(new RoyalMember("James, Earl of Wessex", new DateTime(2007, 12, 17)));

            return root;
        }

        public IActionResult Index()
        {
            var list = new List<FamilyNode>();
            Root.FillPreOrder(list);
            ViewBag.TotalCount = list.Count;
            return View(Root);
        }

        [HttpGet]
        public IActionResult Succession()
        {
            var list = new List<FamilyNode>();
            Root.FillPreOrder(list);
            return View(list);
        }

        [HttpGet]
        public IActionResult Search(string name, string method = "BFS")
        {
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction("Index");

            FamilyNode found;
            List<string> traversal;

            if (method == "DFS")
            {
                var result = DFSSearch(Root, name);
                found = result.found;
                traversal = result.traversal;
            }
            else
            {
                found = BFSSearch(Root, name, out traversal);
            }

            ViewBag.Traversal = traversal;
            ViewBag.Method = method;
            ViewBag.SearchName = name;

            return View("SearchResult", found);
        }

        [HttpPost]
        public IActionResult AddChild(string parentName, RoyalMember child)
        {
            var (found, traversal) = DFSSearch(Root, parentName);
            if (found == null)
            {
                ViewBag.Message = $"Parent '{parentName}' not found.";
                return View("SearchResult", null);
            }

            found.AddChild(child);
            ViewBag.Message = $"{child.Name} successfully added under {found.Member.Name}.";
            ViewBag.Traversal = traversal;

            return RedirectToAction("Index");
        }

        // ----- BFS -----
        private static FamilyNode BFSSearch(FamilyNode root, string name, out List<string> traversal)
        {
            traversal = new();
            var q = new Queue<FamilyNode>();
            q.Enqueue(root);

            while (q.Count > 0)
            {
                var node = q.Dequeue();
                traversal.Add(node.Member.Name);
                if (node.Member.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return node;

                foreach (var child in node.Children)
                    q.Enqueue(child);
            }
            return null;
        }

        // ----- DFS -----
        private static (FamilyNode found, List<string> traversal) DFSSearch(FamilyNode root, string name)
        {
            List<string> traversal = new();
            FamilyNode found = null;

            void dfs(FamilyNode n)
            {
                if (n == null || found != null) return;
                traversal.Add(n.Member.Name);
                if (n.Member.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    found = n;
                    return;
                }
                foreach (var c in n.Children)
                    dfs(c);
            }

            dfs(root);
            return (found, traversal);
        }
    }
}
