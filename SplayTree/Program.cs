using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTree
{
    class Program
    {
        static void Main(string[] args)
        {
        }
        static void SetWithSum2()
        {
            var countQuery = int.Parse(Console.ReadLine());
            StringBuilder sb = new StringBuilder();
            SplayTree splayTree = new SplayTree();
            long sum = 0;
            if (countQuery >= 1 && countQuery <= 100000)
            {
                for (int i = 0; i < countQuery; i++)
                {
                    if (i == 9)
                    {
                        var sd = 5;
                    }
                    var line = Console.ReadLine().Split(' ');
                    var queryCase = line[0];
                    switch (queryCase)
                    {
                        case "+":
                            if (long.Parse(line[1]) >= 0)
                            {
                                var addNumber = Foo(long.Parse(line[1]), sum);;
                                splayTree.Add(addNumber);
                            }
                            break;
                        case "-":
                            if (long.Parse(line[1]) >= 0)
                            {
                                var deleteNumber = Foo(long.Parse(line[1]), sum);
                                splayTree.Delete(deleteNumber);
                            }
                            break;
                        case "?":
                            var searchNumber = Foo(long.Parse(line[1]), sum);
                            if (long.Parse(line[1]) >= 0)
                            {
                                var search = splayTree.Search(searchNumber);
                                if (search)
                                {
                                    sb.Append("Found" + "\n");
                                }
                                else
                                {
                                    sb.Append("Not found" + "\n");
                                }
                            }
                            else
                            {
                                sb.Append("Not found" + "\n");
                            }
                            break;
                        case "s":
                            var leftNumber = Foo(long.Parse(line[1]), sum);
                            var rightNumber = Foo(long.Parse(line[2]), sum);
                            sum = splayTree.Sum(leftNumber, rightNumber);
                            sb.Append(sum + "\n");
                            break;
                    }
                }
                Console.WriteLine(sb);
            }
        }
        static long Foo(long x, long s)
        {
            long result = (x + s) % 1000000001;
            return result;
        }
        public class SplayTree
        {
            class SplayNode
            {
                public SplayNode Left { get; set; }
                public SplayNode Right { get; set; }
                public long Value { get; set; }
                public long Sum { get; set; }
            }

            SplayNode RootNode { get; set; }

            #region add node
            public void Add(long value)
            {
                RootNode = CreateNode(RootNode, value);
            }

            SplayNode CreateNode(SplayNode root, long value)
            {
                // Simple Case: If tree is empty
                if (root == null) return newNode(value);

                // Bring the closest leaf node to root
                root = splay(root, value);

                // If key is already present, then return
                if (root.Value == value) return root;

                // Otherwise allocate memory for new node
                SplayNode newnode = newNode(value);

                // If root's key is greater, make root as right child
                // of newnode and copy the left child of root to newnode
                if (root.Value > value)
                {
                    newnode.Right = root;
                    newnode.Left = root.Left;
                    root.Left = null;
                }

                // If root's key is smaller, make root as left child
                // of newnode and copy the right child of root to newnode
                else
                {
                    newnode.Left = root;
                    newnode.Right = root.Right;
                    root.Right = null;
                }
                newnode.Sum = newnode.Value + GetSum(newnode.Left) + GetSum(newnode.Right);
                return newnode; // newnode becomes new root
            }
            #endregion

            #region delete node
            public void Delete(long value)
            {
                RootNode = RemoveNode(RootNode, value);
            }
            SplayNode RemoveNode(SplayNode root, long value)
            {
                if (root != null)
                {
                    var searchFlag = Search(value);
                    if (searchFlag)
                    {
                        RootNode = splay(RootNode, value);
                        return merge(RootNode.Left, RootNode.Right);
                    }
                    else
                    {
                        return RootNode;
                    }
                }
                else
                {
                    return root;
                }
            }
            #endregion

            #region search node
            public bool Search(long value)
            {
                RootNode = search(value);

                if (RootNode != null)
                {
                    return RootNode.Value == value ? true : false;
                }
                else
                {
                    return false;
                }
            }
            SplayNode search(long value)
            {
                RootNode = splay(RootNode, value);
                return RootNode;
            }
            #endregion

            #region sum
            public long Sum(long leftValue, long rightValue)
            {
                return SumOfSet(RootNode, leftValue, rightValue);
            }
            long SumOfSet(SplayNode root, long leftValue, long rightValue)
            {
                SplayNode left = new SplayNode();
                SplayNode middle = new SplayNode();
                SplayNode right = new SplayNode();
                long result = 0;
                var splitLeft = split(root, leftValue - 1);
                left = splitLeft.Item1;
                middle = splitLeft.Item2;

                var splitRight = split(middle, rightValue + 1);
                middle = splitRight.Item1;
                right = splitRight.Item2;

                if (middle != null)
                {
                    result += middle.Sum;
                }

                SplayNode newMiddle = merge(left, middle);
                RootNode = merge(newMiddle, right);

                return result;
            }
            #endregion

            #region utils
            SplayNode newNode(long value)
            {
                SplayNode node = new SplayNode();
                node.Value = value;
                node.Left = null;
                node.Right = null;
                node.Sum = node.Value;
                return (node);
            }
            SplayNode rightRotate(SplayNode node)
            {
                SplayNode temp = node.Left;
                node.Left = temp.Right;
                node.Sum = node.Value + GetSum(node.Left) + GetSum(node.Right);
                temp.Right = node;
                temp.Sum = temp.Value + GetSum(temp.Left) + GetSum(temp.Right);
                return temp;
            }
            SplayNode leftRotate(SplayNode node)
            {
                SplayNode temp = node.Right;
                node.Right = temp.Left;
                node.Sum = node.Value + GetSum(node.Left) + GetSum(node.Right);
                temp.Left = node;
                temp.Sum = temp.Value + GetSum(temp.Left) + GetSum(temp.Right);
                return temp;
            }
            SplayNode splay(SplayNode root, long value)
            {
                // Base cases: root is NULL or key is present at root
                if (root == null || root.Value == value)
                    return root;

                // Key lies in left subtree
                if (root.Value > value)
                {
                    // Key is not in tree, we are done
                    if (root.Left == null) return root;

                    // Zig-Zig (Left Left)
                    if (root.Left.Value > value)
                    {
                        // First recursively bring the key as root of left-left
                        root.Left.Left = splay(root.Left.Left, value);

                        // Do first rotation for root, second rotation is done after else
                        root = rightRotate(root);
                    }
                    else if (root.Left.Value < value) // Zig-Zag (Left Right)
                    {
                        // First recursively bring the key as root of left-right
                        root.Left.Right = splay(root.Left.Right, value);

                        // Do first rotation for root->left
                        if (root.Left.Right != null)
                            root.Left = leftRotate(root.Left);
                    }

                    // Do second rotation for root
                    return (root.Left == null) ? root : rightRotate(root);
                }
                else // Key lies in right subtree
                {
                    // Key is not in tree, we are done
                    if (root.Right == null) return root;

                    // Zag-Zig (Right Left)
                    if (root.Right.Value > value)
                    {
                        // Bring the key as root of right-left
                        root.Right.Left = splay(root.Right.Left, value);

                        // Do first rotation for root->right
                        if (root.Right.Left != null)
                            root.Right = rightRotate(root.Right);
                    }
                    else if (root.Right.Value < value)// Zag-Zag (Right Right)
                    {
                        // Bring the key as root of right-right and do first rotation
                        root.Right.Right = splay(root.Right.Right, value);
                        root = leftRotate(root);
                    }

                    // Do second rotation for root
                    return (root.Right == null) ? root : leftRotate(root);
                }
            }
            Tuple<SplayNode, SplayNode> split(SplayNode root, long value)
            {
                if (root == null)
                    return new Tuple<SplayNode, SplayNode>(null, null);

                root = splay(root, value);

                if (root.Value == value)
                {
                    root.Sum = root.Value + GetSum(root.Left) + GetSum(root.Right);
                    return new Tuple<SplayNode, SplayNode>(root.Left, root.Right);
                }
                else if (root.Value < value)
                {
                    var tempRight = root.Right;
                    root.Right = null;

                    root.Sum = root.Value + GetSum(root.Left) + GetSum(root.Right);
                    if (tempRight != null)
                    {
                        tempRight.Sum = tempRight.Value + GetSum(tempRight.Left) + GetSum(tempRight.Right);
                    }

                    return new Tuple<SplayNode, SplayNode>(root, tempRight);
                }
                else
                {
                    var tempLeft = root.Left;
                    root.Left = null;

                    if (tempLeft != null)
                    {
                        tempLeft.Sum = tempLeft.Sum + GetSum(tempLeft.Left) + GetSum(tempLeft.Right);
                    }
                    root.Sum = root.Sum + GetSum(root.Left) + GetSum(root.Right);

                    return new Tuple<SplayNode, SplayNode>(tempLeft, root);
                }
            }
            SplayNode merge(SplayNode left, SplayNode right)
            {
                if (right == null) return left;
                if (left == null) return right;

                right = splay(right, left.Value);
                right.Left = left;

                right.Sum = right.Value + GetSum(right.Left) + GetSum(right.Right);

                return right;
            }
            long GetSum(SplayNode node)
            {
                return node != null ? node.Sum : 0;
            }

            #endregion

        }
    }
}
