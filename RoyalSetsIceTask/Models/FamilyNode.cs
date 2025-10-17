namespace RoyalSetsIceTask.Models
{
    public class FamilyNode
    {
        public RoyalMember Member { get; set; }
        public List<FamilyNode> Children { get; set; } = new();
        public FamilyNode Parent { get; set; }

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
    }
}
