using System;
using System.Collections.Generic;

namespace JPEG_Encoder.encoding.huffman.model
{
    public class HuffmanTree
    {
        private HuffmanTreeComponent _root;
        private readonly List<HuffmanTreeLeaf> _symbols;
        private bool _fullBitEliminated;

        public HuffmanTree(HuffmanTreeComponent root, List<HuffmanTreeLeaf> symbols)
        {
            _root = root;
            _symbols = symbols;
        }

        public void MakeCanonical()
        {
            Dictionary<int, List<HuffmanTreeComponent>> tree = new Dictionary<int, List<HuffmanTreeComponent>>();
            LayerTreeAndSortNodesByDepth(tree);
            RecompositeTree(tree);
        }

        private void LayerTreeAndSortNodesByDepth(Dictionary<int,List<HuffmanTreeComponent>> tree)
        {
            List<HuffmanTreeComponent> nodesOfCurrentLevel = new List<HuffmanTreeComponent>();
            nodesOfCurrentLevel.Add(_root);
            for (int i = 0; i <= GetDepth(); i++)
            {
                List<HuffmanTreeComponent> nodesOfNextLevel = new List<HuffmanTreeComponent>();
                tree.Add(i, new List<HuffmanTreeComponent>());
                foreach (HuffmanTreeComponent currentNode in nodesOfCurrentLevel)
                {
                    tree[i].Add(currentNode);
                    if (currentNode.GetLeft() != null)
                    {
                        nodesOfNextLevel.Add(currentNode.GetLeft());
                    }

                    if (currentNode.GetRight() != null)
                    {
                        nodesOfNextLevel.Add(currentNode.GetRight());
                    }
                }

                nodesOfNextLevel.Sort(new DepthComparator());
                nodesOfCurrentLevel = nodesOfNextLevel;
            }
        }

        private void RecompositeTree(IReadOnlyDictionary<int, List<HuffmanTreeComponent>> tree)
        {
            List<HuffmanTreeComponent> nodesOfCurrentLevel;
            for (int i =  tree.Count - 1; i > 0; i--)
            {
                nodesOfCurrentLevel = tree[i];
                List<HuffmanTreeComponent> nodesOfPreviousLevel = tree[i - 1];
                int finishedNodes = 0;
                for (int j = nodesOfCurrentLevel.Count - 1; j > 0; j = j - 2)
                {
                    HuffmanTreeComponent lastUnusedNode =
                        nodesOfPreviousLevel[nodesOfPreviousLevel.Count - finishedNodes++ - 1];
                    lastUnusedNode.SetLeft(nodesOfCurrentLevel[j-1]);
                    lastUnusedNode.SetRight(nodesOfCurrentLevel[j]);
                    
                }
            }
        }

        public void ReplaceMostRight()
        {
            if (_fullBitEliminated) return;
            HuffmanTreeComponent currentNode = _root;
            HuffmanTreeComponent previousNode = _root;
            while (currentNode.GetRight() != null)
            {
                previousNode = currentNode;
                currentNode = currentNode.GetRight();
            }
            previousNode.SetRight(new HuffmanTreeNode(currentNode, new HuffmanTreeNullLeaf()));
            _symbols.Add(new HuffmanTreeNullLeaf());
            _fullBitEliminated = true;
        }

        public void RestrictToLength(int restriction)
        {
            if (ValidateRestriction(restriction))
            {
                Dictionary<int, List<HuffmanTreeComponent>> coinDrawers = InitCoinDrawers(restriction);
                PackageMerge(restriction, coinDrawers);
                Dictionary<HuffmanTreeLeaf, int> codeWordLengths = Evaluate(restriction, coinDrawers);
                _root = CreateLengthLimitedTree(codeWordLengths, restriction);
            }
        }

        private Dictionary<HuffmanTreeLeaf, int> Evaluate(int restriction, Dictionary<int, List<HuffmanTreeComponent>> coinDrawers)
        {
            Dictionary<HuffmanTreeLeaf, int> codeWordLengths = new Dictionary<HuffmanTreeLeaf, int>();
            for (int denominationPower = -restriction; denominationPower < 0; denominationPower++)
            {
                List<HuffmanTreeComponent> currentDrawer = coinDrawers[denominationPower];
                foreach (HuffmanTreeComponent coin in currentDrawer)
                {
                    if (coin.GetType().IsAssignableFrom(typeof(HuffmanTreeLeaf)))
                    {
                        HuffmanTreeLeaf symbol = (HuffmanTreeLeaf) coin;
                        if (codeWordLengths.ContainsKey(symbol))
                        {
                            codeWordLengths.Add(symbol, codeWordLengths[symbol] + 1);
                        }
                        else
                        {
                            codeWordLengths.Add(symbol, 1);
                        }
                    }
                }
            }
            return codeWordLengths;
        }
        
        private HuffmanTreeComponent CreateLengthLimitedTree(Dictionary<HuffmanTreeLeaf,int> codeWordLengths, int restriction)
        {
            Dictionary<int, List<HuffmanTreeComponent>> tree = new Dictionary<int, List<HuffmanTreeComponent>>();
            PrepareLevels(restriction, tree);
            FillLevelsWithInitialNodes(codeWordLengths, tree);
            RecreateTree(restriction, tree);
            return tree[0][0];
        }

        private void PrepareLevels(int restriction, Dictionary<int,List<HuffmanTreeComponent>> tree)
        {
            for (int i = 0; i <= restriction; i++)
            {
                tree.Add(i, new List<HuffmanTreeComponent>());
            }
        }
        
        private void FillLevelsWithInitialNodes(Dictionary<HuffmanTreeLeaf, int> codeWordLengths, IReadOnlyDictionary<int, List<HuffmanTreeComponent>> tree)
        {
            foreach (KeyValuePair<HuffmanTreeLeaf, int> entry in codeWordLengths)
            {
                tree[entry.Value].Add(entry.Key);
            }
        }
        
        private void RecreateTree(int restriction, Dictionary<int, List<HuffmanTreeComponent>> tree)
        {
            for (int i = restriction; i > 0; i--)
            {
                List<HuffmanTreeComponent> currentLevel = tree[i];
                List<HuffmanTreeComponent> nextLevel = tree[i - 1];
                currentLevel.Sort(new DepthComparator());
                for (int j = currentLevel.Count - 1; j > 0; j = j - 2)
                {
                    HuffmanTreeComponent newNode = new HuffmanTreeNode(currentLevel[j - 1], currentLevel[j]);
                    nextLevel.Add(newNode);
                }
            }
        }

        private bool ValidateRestriction(int restriction)
        {
            if (this.GetDepth() <= restriction)
            {
                return false;
            }

            if (Math.Ceiling(Math.Log(GetNumOfSymbols()) / Math.Log(2)) > restriction)
            {
                return false;
            }

            return true;
        }

        private void PackageMerge(int restriction, Dictionary<int,List<HuffmanTreeComponent>> coinDrawers)
        {
            for (int denominationPower = -restriction; denominationPower < 0; denominationPower++)
            {
                List<HuffmanTreeComponent> currentDrawer = coinDrawers[denominationPower];
                currentDrawer.Sort();
                if ((currentDrawer.Count % 2) != 0)
                {
                    RemoveNodeAndItsChildren(coinDrawers, currentDrawer, denominationPower);
                }

                for (int i = 0; i < currentDrawer.Count; i = i + 2)
                {
                    coinDrawers[denominationPower + 1].Add(new HuffmanTreeNode(currentDrawer[i], currentDrawer[i + 1]));
                }
            }
        }

        private void RemoveNodeAndItsChildren(Dictionary<int,List<HuffmanTreeComponent>> coinDrawers, List<HuffmanTreeComponent> currentDrawer, int currentDenominationPower)
        {
            // lösche Kinder rekursiv
            if (!currentDrawer[currentDrawer.Count - 1].GetType().IsAssignableFrom(typeof(HuffmanTreeLeaf)))
            {
                List<HuffmanTreeComponent> previousDrawer = coinDrawers[currentDenominationPower - 1];
                RemoveNodeAndItsChildren(coinDrawers, previousDrawer, currentDenominationPower - 1);
                RemoveNodeAndItsChildren(coinDrawers, previousDrawer, currentDenominationPower - 1);
            }
            // lösche aktueller Knoten
            currentDrawer.RemoveAt(currentDrawer.Count - 1);
        }

        private Dictionary<int, List<HuffmanTreeComponent>> InitCoinDrawers(int restriction)
        {
            Dictionary<int, List<HuffmanTreeComponent>> coinDrawer = new Dictionary<int, List<HuffmanTreeComponent>>();
            coinDrawer.Add(0, new List<HuffmanTreeComponent>());
            for (int i = -restriction; i < 0; i++)
            {
                coinDrawer.Add(i, new List<HuffmanTreeComponent>());
                foreach (HuffmanTreeLeaf leaf in _symbols)
                {
                    coinDrawer[i].Add(leaf);
                }
            }

            return coinDrawer;
        }

        public List<CodeWord> GetCodeBook()
        {
            List<CodeWord> codeWords = new List<CodeWord>((int) Math.Pow(2, GetDepth()));
            _root.FillCodeBook(codeWords, 0, 0);
            return codeWords;
        }

        public Dictionary<int, CodeWord> GetCodeBookAsDictionary()
        {
            Dictionary<int, CodeWord> codeWordMap = new Dictionary<int, CodeWord>();
            foreach (CodeWord codeWord in GetCodeBook())
            {
                codeWordMap.Add(codeWord.GetSymbol(), codeWord);
            }

            return codeWordMap;
        }
        
        private int GetNumOfSymbols()
        {
            return _symbols.Count;
        }
        
        private int GetDepth()
        {
            return _root.GetDepth(0);
        }

        public void PrintCodes()
        {
            _root.PrintCode("");
        }

        public override string ToString()
        {
            return _root.ToString();
        }
    }
}