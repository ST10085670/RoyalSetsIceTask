namespace RoyalSetsIceTask.Models
{
    public class FamilyNode
    {
        public RoyalMember Member { get; set; }
        public List<FamilyNode> Children { get; set; } = new();
        public FamilyNode Parent { get; set; }

        // New property to mark a node as highlighted
        public bool IsHighlighted { get; set; } = false;

        public FamilyNode() { }
        public FamilyNode(RoyalMember member) => Member = member;

        public FamilyNode AddChild(RoyalMember child)
        {
            var node = new FamilyNode(child) { Parent = this };
            Children.Add(node);
            return node;
        }

        public void FillPreOrder(List<FamilyNode> list)
        {
            list.Add(this);
            foreach (var c in Children.OrderBy(c => c.Member.DateOfBirth))
                c.FillPreOrder(list);
        }

        public List<FamilyNode> GetPathToRoot()
        {
            var path = new List<FamilyNode>();
            var cur = this;
            while (cur != null)
            {
                path.Add(cur);
                cur = cur.Parent;
            }
            path.Reverse();
            return path;
        }

        // ------------------ Search ------------------
        public FamilyNode SearchMember(string query, string method = "BFS")
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            query = query.Trim().ToLower();

            // Clear previous highlights
            ClearHighlights(this);

            FamilyNode found = method.ToUpper() == "DFS" ? DFS(this, query) : BFS(this, query);

            if (found != null)
            {
                found.IsHighlighted = true;
            }

            return found;
        }

        // Clear any previous highlights recursively
        private void ClearHighlights(FamilyNode node)
        {
            if (node == null) return;
            node.IsHighlighted = false;
            foreach (var child in node.Children)
                ClearHighlights(child);
        }

        // Depth-First Search with partial, case-insensitive match
        private FamilyNode DFS(FamilyNode node, string query)
        {
            if (node == null) return null;
            if (node.Member.Name.ToLower().Contains(query)) return node;

            foreach (var child in node.Children)
            {
                var found = DFS(child, query);
                if (found != null) return found;
            }

            return null;
        }

        // Breadth-First Search with partial, case-insensitive match
        private FamilyNode BFS(FamilyNode root, string query)
        {
            var queue = new Queue<FamilyNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Member.Name.ToLower().Contains(query)) return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }

            return null;
        }
    }
}
