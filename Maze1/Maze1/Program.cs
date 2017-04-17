using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Maze1
{
    /// <summary>
    /// Main program to maze solving functionality
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main Function of maze solving program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Saving start time for response time calculations
            DateTime startTime = DateTime.Now;

            // Initiate Node information dictionary to be used as tree for easy data manipulation
            Dictionary<Node, List<Node>> nodesInformation = new Dictionary<Node, List<Node>>();

            // Save Console Input
            string[] parameters = new string[3];
            try
            {
                parameters = Console.ReadLine().Split(null).Select(x => x.Replace("\"", "")).ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while taking input. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Check input syntax calling maze.exe
            if (parameters[0] != "maze.exe")
            {
                Console.WriteLine("The program called should be maze.exe. E.g. maze.exe “Source path” “destination path”." +
                    " Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Check if input parameter is valid
            if (parameters[1].Split('.')[1] != "bmp" && parameters[1].Split('.')[1] != "png" && parameters[1].Split('.')[1] != "jpg")
            {
                Console.WriteLine("The source file should be either .bmp, .png or .jpg. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (!File.Exists(parameters[1]))
            {
                Console.WriteLine("The input file location is not valid. Please check if file exists and provide proper file location." +
                    " Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Checks if output file extension is valid
            if (parameters[2].Split('.')[1] != "bmp" && parameters[2].Split('.')[1] != "png" && parameters[2].Split('.')[1] != "jpg")
            {
                Console.WriteLine("The destination file should be either .bmp, .png or .jpg. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Check if output directory path exists
            try
            {
                // If the directory doesn't exist, create it.
                if (!Directory.Exists(parameters[2].Substring(0, parameters[2].LastIndexOf('\\'))))
                {
                    Directory.CreateDirectory(parameters[2].Substring(0, parameters[2].LastIndexOf('\\')));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while creating desired destination path. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Convert source image to 2D integer array
            Console.WriteLine("2D array generation started at {0}", (DateTime.Now - startTime));

            // Initiate corresponding 2D integer array for use throughout the application
            int[,] imgArray = ImageHelper.Image2DIntArrayConverter(parameters[1]);
            Console.WriteLine("2D array generated at {0}", (DateTime.Now - startTime));

            // Find root node i.e. the a pixel from the red source
            Console.WriteLine("Root and Destination search started at {0}", (DateTime.Now - startTime));
            Tuple<Node, Node> SourceAndDestination = FindRootAndDestinationNode(imgArray);
            Node rootNode = SourceAndDestination.Item1;

            // Check if start node is present
            if (rootNode != null)
            {
                // Adding root node in nodesInformation dictionary
                nodesInformation.Add(rootNode, new List<Node>());
                Console.WriteLine("Root Found at {0}", (DateTime.Now - startTime));
            }
            else
            {
                Console.WriteLine("The start point is not marked in red. Please mark start point with color Red. Press Enter to exit application....");
                Environment.Exit(0);
            }

            // Check if destination node is present
            Node destinationNode = SourceAndDestination.Item2;
            if (destinationNode != null)
            {
                // Adding destination node in nodesInformation dictionary
                nodesInformation.Add(destinationNode, new List<Node>());
                Console.WriteLine("Destination Found at {0}", (DateTime.Now - startTime));
            }
            else
            {
                Console.WriteLine("The end pont is not marked in blue. Please mark destination point with color Blue. Press Enter to exit application....");
            }

            // Find all corner pixel nodes across the image used for navigation
            Console.WriteLine("Corner nodes search started at {0}", (DateTime.Now - startTime));
            FindNavigationalCornerNodes(imgArray, nodesInformation);
            Console.WriteLine("Corner nodes found at {0}", (DateTime.Now - startTime));

            // Populate navigational nodes from nodesInformation dictionary on basis of Line Of Sight
            Console.WriteLine("Node information dictionary population started at {0}", (DateTime.Now - startTime));
            PopulateNodesInformationtree(imgArray, nodesInformation);
            Console.WriteLine("Node information dictionary populated at {0}", (DateTime.Now - startTime));

            // Relate nodes from source to destination using breadth first search on nodesInformation tree to find shortest path
            Console.WriteLine("Relationships assignment till destination node started at {0}", (DateTime.Now - startTime));
            AssignRelationshipsUsingBreadthFirstSearch(nodesInformation, rootNode, destinationNode);
            Console.WriteLine("Relationships assigned till destination node at {0}", (DateTime.Now - startTime));

            // Save path from destination to source for tracer
            // Initiate optimal path stack to save the optimal path using parent property of the node class
            Stack<Node> optimalPath = new Stack<Node>();
            Console.WriteLine("Optimal path saving started at {0}", (DateTime.Now - startTime));
            SaveOptimalPath(destinationNode, rootNode, optimalPath);
            Console.WriteLine("Optimal path saved at {0}", (DateTime.Now - startTime));

            // Tracing green path from destination to source using the optimal path
            Console.WriteLine("Green path tracing started at {0}", (DateTime.Now - startTime));
            Tracer(imgArray, optimalPath, destinationNode);
            Console.WriteLine("Green path traced at {0}", (DateTime.Now - startTime));

            // Checking the extension desired for the output file and Saving the image in output file in the directory mentioned in parameters
            // Convert the 2D integer array to the destination image
            Console.WriteLine("Destination image with optimal path construction started at {0}", (DateTime.Now - startTime));
            ImageHelper.MakeandSaveImageinDesiredFormat(parameters[2], imgArray);
            Console.WriteLine("Destination image made at {0}", (DateTime.Now - startTime));
            Console.ReadLine();
        }

        /// <summary>
        /// Function to Save path from destination to source for tracer
        /// </summary>
        /// <param name="destinationNode"></param>
        /// <param name="rootNode"></param>
        /// <param name="optimalPath"></param>
        private static void SaveOptimalPath(Node destinationNode, Node rootNode, Stack<Node> optimalPath)
        {
            try
            {
                // Start saving from the destination node to the root node
                optimalPath.Push(destinationNode);
                do
                {
                    // Finding immediate parent for previous node
                    Node parentNode = optimalPath.Peek().Parent;
                    if (parentNode != null)
                    {
                        optimalPath.Push(parentNode);
                    }
                } while (optimalPath.Peek() != rootNode);
            }catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while saving optimal path from source to destination. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Function for Tracing green path from destination to source using the optimal path
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="optimalPath"></param>
        /// <param name="destinationNode"></param>
        private static void Tracer(int[,] imgArray, Stack<Node> optimalPath, Node destinationNode)
        {
            try
            {
                // Initialze starting node with the root node
                Node currentNode = optimalPath.Pop();
                // Sight used to trace green path between two nodes
                Point sight = new Point();
                // Next denoted the intermidiate destination node for trace direction
                Point next = new Point();
                // Start tracing till we reach the destination node
                do
                {
                    // Reuse sight and next for each node pairs path tracing
                    sight = new Point(currentNode.Position.X, currentNode.Position.Y);
                    next = new Point(optimalPath.Peek().Position.X, optimalPath.Peek().Position.Y);
                    // Variables for trace direction judgment
                    int horizontal, vertical;
                    do
                    {
                        horizontal = next.X - sight.X;
                        vertical = next.Y - sight.Y;
                        // Traverse up
                        if (horizontal == 0 && vertical > 0)
                        {
                            sight.Offset(0, 1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse right and up
                        else if (horizontal > 0 && vertical > 0)
                        {
                            sight.Offset(1, 1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse right
                        else if (horizontal > 0 && vertical == 0)
                        {
                            sight.Offset(1, 0);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse right and down
                        else if (horizontal > 0 && vertical < 0)
                        {
                            sight.Offset(1, -1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse down
                        else if (horizontal == 0 && vertical < 0)
                        {
                            sight.Offset(0, -1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse left and down
                        else if (horizontal < 0 && vertical < 0)
                        {
                            sight.Offset(-1, -1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse left
                        else if (horizontal < 0 && vertical == 0)
                        {
                            sight.Offset(-1, 0);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                        // Traverse left and up
                        else if (horizontal < 0 && vertical > 0)
                        {
                            sight.Offset(-1, 1);
                            imgArray[sight.X, sight.Y] = 4;
                        }
                    } while (sight.X != next.X || sight.Y != next.Y);
                    currentNode = optimalPath.Pop();
                } while (currentNode != destinationNode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + " occurred while tracing green path between source and destination nodes. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Function for Relating nodes from source to destination using breadth first search on nodesInformation tree to find shortest path
        /// </summary>
        /// <param name="nodesInformation"></param>
        /// <param name="rootNode"></param>
        /// <param name="destinationNode"></param>
        private static void AssignRelationshipsUsingBreadthFirstSearch(Dictionary<Node, List<Node>> nodesInformation, Node rootNode, Node destinationNode)
        {
            try
            {
                // Initialize relationshipQueue to facilitate Breadth-First-Search on the nodesInformation tree to find shortest path
                Queue<Node> relationshipQueue = new Queue<Node>();
                // Start from the root node
                relationshipQueue.Enqueue(rootNode);
                // Loop until destination node is reached
                do
                {
                    Node currentNode = relationshipQueue.Dequeue();
                    if (nodesInformation[currentNode].Count > 0)
                    {
                        // Enlist nodes visible from the current node
                        foreach (Node child in nodesInformation[currentNode])
                        {
                            if (child.Parent == null)
                            {
                                // Assign current node as parent of the child node
                                child.Parent = currentNode;
                                if (child == destinationNode)
                                {
                                    currentNode = destinationNode;
                                    break;
                                }
                                // Enqueue child nodes in the queue to find their child nodes
                                relationshipQueue.Enqueue(child);
                            }
                        }
                    }
                    if (currentNode == destinationNode)
                    {
                        // Break from the loop once the destination node is found
                        break;
                    }
                } while (relationshipQueue.Count() != 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while assigning node relationships using breadth first search. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Function to Populate navigational nodes from nodesinformation dictionary on basis of Line Of Sight
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="nodesInformation"></param>
        private static void PopulateNodesInformationtree(int[,] imgArray, Dictionary<Node, List<Node>> nodesInformation)
        {
            try
            {
                // Reducing the scope of scanning for related nodes to 1/16th of the actual image size
                int quarterArrayWidth = imgArray.GetLength(0) / 4, quarterArrayHeight = imgArray.GetLength(1) / 4, horizontal, vertical;
                // Initializing neighbour nodes list for use in loop
                List<Node> neighbourNodes = new List<Node>();
                // IsVisible flag used to determine if a given node is visible and reachable from the current node in consideration
                bool isVisible;
                // Initializing sight for tracing implementation
                Point sight = new Point();

                foreach (Node node1 in nodesInformation.Keys)
                {
                    // Reducing the scope of scanning for related nodes to 1/16th of the actual image size
                    neighbourNodes = nodesInformation.Keys.
                        Where(x => x != node1 &&
                        x.Position.X < node1.Position.X + quarterArrayWidth && x.Position.X > node1.Position.X - quarterArrayWidth &&
                        x.Position.Y < node1.Position.Y + quarterArrayHeight && x.Position.Y > node1.Position.Y - quarterArrayHeight).ToList();

                    // Traverse to each neighbouring node check if they fall under the Line of Sight
                    foreach (Node node2 in neighbourNodes)
                    {
                        isVisible = false;
                        sight = new Point(node1.Position.X, node1.Position.Y);
                        do
                        {
                            horizontal = node2.Position.X - sight.X;
                            vertical = node2.Position.Y - sight.Y;
                            // Traverse up
                            if (horizontal == 0 && vertical > 0)
                            {
                                sight.Offset(0, 1);
                                if (imgArray[sight.X, sight.Y] == 1)
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse right and up
                            else if (horizontal > 0 && vertical > 0)
                            {
                                if (imgArray[sight.X + 1, sight.Y] == 0 || imgArray[sight.X, sight.Y + 1] == 0)
                                {
                                    sight.Offset(1, 1);
                                    if (imgArray[sight.X, sight.Y] == 1)
                                    {
                                        isVisible = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse right
                            else if (horizontal > 0 && vertical == 0)
                            {
                                sight.Offset(1, 0);
                                if (imgArray[sight.X, sight.Y] == 1)
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse right and down
                            else if (horizontal > 0 && vertical < 0)
                            {
                                if (imgArray[sight.X + 1, sight.Y] == 0 || imgArray[sight.X, sight.Y - 1] == 0)
                                {
                                    sight.Offset(1, -1);
                                    if (imgArray[sight.X, sight.Y] == 1)
                                    {
                                        isVisible = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse down
                            else if (horizontal == 0 && vertical < 0)
                            {
                                sight.Offset(0, -1);
                                if (imgArray[sight.X, sight.Y] == 1)
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse left and down
                            else if (horizontal < 0 && vertical < 0)
                            {
                                if (imgArray[sight.X - 1, sight.Y] == 0 || imgArray[sight.X, sight.Y - 1] == 0)
                                {
                                    sight.Offset(-1, -1);
                                    if (imgArray[sight.X, sight.Y] == 1)
                                    {
                                        isVisible = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse left
                            else if (horizontal < 0 && vertical == 0)
                            {
                                sight.Offset(-1, 0);
                                if (imgArray[sight.X, sight.Y] == 1)
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // Traverse left and up
                            else if (horizontal < 0 && vertical > 0)
                            {
                                if (imgArray[sight.X - 1, sight.Y] == 0 || imgArray[sight.X, sight.Y + 1] == 0)
                                {
                                    sight.Offset(-1, 1);
                                    if (imgArray[sight.X, sight.Y] == 1)
                                    {
                                        isVisible = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    isVisible = false;
                                    break;
                                }
                            }
                            // If sight reaches the target node
                            if (sight.X == node2.Position.X || sight.Y == node2.Position.Y)
                            {
                                isVisible = true;
                            }
                        } while (sight.X != node2.Position.X || sight.Y != node2.Position.Y);
                        // If node is traversable, then add the node to the corresponding current nodes entry in the nodesInformation dictionary
                        if (isVisible)
                        {
                            nodesInformation[node1].Add(node2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while populating node information tree. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Function to Find all corner pixel nodes across the image used for navigation
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="nodesInformation"></param>
        private static void FindNavigationalCornerNodes(int[,] imgArray, Dictionary<Node, List<Node>> nodesInformation)
        {
            try
            {
                int arrayWidth = imgArray.GetLength(0), arrayHeight = imgArray.GetLength(1);

                for (var i = 1; i < arrayWidth - 1; i++)
                {
                    for (var j = 1; j < arrayHeight - 1; j++)
                    {
                        if (imgArray[i, j] == 1)
                        {
                            // Right-Bottom corner node
                            if (imgArray[i, j - 1] == 0 && imgArray[i + 1, j - 1] == 0 && imgArray[i + 1, j] == 0 &&
                                nodesInformation.Count(x => x.Key.Position.X == i + 1 && x.Key.Position.Y == j - 1) == 0)
                            {
                                nodesInformation.Add(new Node(i + 1, j - 1), new List<Node>());
                            }
                            // Right-Top corner node
                            if (imgArray[i + 1, j] == 0 && imgArray[i, j + 1] == 0 && imgArray[i + 1, j + 1] == 0
                                && nodesInformation.Count(x => x.Key.Position.X == i + 1 && x.Key.Position.Y == j + 1) == 0)
                            {
                                nodesInformation.Add(new Node(i + 1, j + 1), new List<Node>());
                            }
                            // Left-Top corner node
                            if (imgArray[i, j + 1] == 0 && imgArray[i - 1, j + 1] == 0 && imgArray[i - 1, j] == 0
                                && nodesInformation.Count(x => x.Key.Position.X == i - 1 && x.Key.Position.Y == j + 1) == 0)
                            {
                                nodesInformation.Add(new Node(i - 1, j + 1), new List<Node>());
                            }
                            // Left-bottom corner node
                            if (imgArray[i - 1, j] == 0 && imgArray[i - 1, j - 1] == 0 && imgArray[i, j - 1] == 0
                                && nodesInformation.Count(x => x.Key.Position.X == i - 1 && x.Key.Position.Y == j - 1) == 0)
                            {
                                nodesInformation.Add(new Node(i - 1, j - 1), new List<Node>());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while finding navigational corner nodes. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        ///  Function to Find root node i.e. the first white pixel just adjecent to the red source pixel and destination node 
        ///  i.e. the first white pixel just adjecent to the blue destination pixel
        /// </summary>
        /// <param name="imgArray"></param>
        /// <returns></returns>
        private static Tuple<Node, Node> FindRootAndDestinationNode(int[,] imgArray)
        {
            try
            {
                int bitmapWidth = imgArray.GetLength(0), bitmapHeight = imgArray.GetLength(1);
                bool sourceFound = false, destinationFound = false;
                Node source = new Node(), destination = new Node();

                for (var i = 0; i < bitmapWidth; i++)
                {
                    for (var j = 0; j < bitmapHeight; j++)
                    {
                        if (!sourceFound && imgArray[i, j] == 2)
                        {
                            // Right-Bottom corner
                            if (imgArray[i + 1, j - 1] == 0)
                            {
                                source = new Node(i + 1, j - 1) { Parent = null };
                                sourceFound = true;
                            }
                            // Right
                            else if (imgArray[i + 1, j] == 0)
                            {
                                source = new Node(i + 1, j) { Parent = null };
                                sourceFound = true;
                            }
                            // Right-Top corner
                            else if (imgArray[i + 1, j + 1] == 0)
                            {
                                source = new Node(i + 1, j + 1) { Parent = null };
                                sourceFound = true;
                            }
                            // Bottom
                            else if (imgArray[i, j - 1] == 0)
                            {
                                source = new Node(i, j - 1) { Parent = null };
                                sourceFound = true;
                            }
                            // Top
                            else if (imgArray[i, j + 1] == 0)
                            {
                                source = new Node(i, j + 1) { Parent = null };
                                sourceFound = true;
                            }
                            // Left-Bottom corner
                            else if (imgArray[i - 1, j - 1] == 0)
                            {
                                source = new Node(i - 1, j - 1) { Parent = null };
                                sourceFound = true;
                            }
                            // Left
                            else if (imgArray[i - 1, j] == 0)
                            {
                                source = new Node(i - 1, j) { Parent = null };
                                sourceFound = true;
                            }
                            // Left-Top corner
                            else if (imgArray[i - 1, j + 1] == 0)
                            {
                                source = new Node(i - 1, j + 1) { Parent = null };
                                sourceFound = true;
                            }
                        }

                        if (!destinationFound && imgArray[i, j] == 3)
                        {
                            // Right-Bottom corner
                            if (imgArray[i + 1, j - 1] == 0)
                            {
                                destination = new Node(i + 1, j - 1) { Parent = null };
                                destinationFound = true;
                            }
                            // Right
                            else if (imgArray[i + 1, j] == 0)
                            {
                                destination = new Node(i + 1, j) { Parent = null };
                                destinationFound = true;
                            }
                            // Right-Top corner
                            else if (imgArray[i + 1, j + 1] == 0)
                            {
                                destination = new Node(i + 1, j + 1) { Parent = null };
                                destinationFound = true;
                            }
                            // Bottom
                            else if (imgArray[i, j - 1] == 0)
                            {
                                destination = new Node(i, j - 1) { Parent = null };
                                destinationFound = true;
                            }
                            // Top
                            else if (imgArray[i, j + 1] == 0)
                            {
                                destination = new Node(i, j + 1) { Parent = null };
                                destinationFound = true;
                            }
                            // Left-Bottom corner
                            else if (imgArray[i - 1, j - 1] == 0)
                            {
                                destination = new Node(i - 1, j - 1) { Parent = null };
                                destinationFound = true;
                            }
                            // Left
                            else if (imgArray[i - 1, j] == 0)
                            {
                                destination = new Node(i - 1, j) { Parent = null };
                                destinationFound = true;
                            }
                            // Left-Top corner
                            else if (imgArray[i - 1, j + 1] == 0)
                            {
                                destination = new Node(i - 1, j + 1) { Parent = null };
                                destinationFound = true;
                            }
                        }
                    }
                }
                return Tuple.Create(source, destination);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + ", occurred while finding source and destination nodes. Press enter to close application....");
                Console.ReadLine();
                Environment.Exit(0);
                return null;
            }
        }
    }
}