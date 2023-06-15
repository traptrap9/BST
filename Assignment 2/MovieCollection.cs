// CAB301 - assignment 2
// An implementation of MovieCollection ADT
// 2023


using System;

//A class that models a node of a binary search tree
//An instance of this class is a node in the binary search tree 
public class BTreeNode
{
    private IMovie movie; // movie
    private BTreeNode? lchild; // reference to its left child 
    private BTreeNode? rchild; // reference to its right child

    public BTreeNode(IMovie movie)
    {
        this.movie = movie;
        lchild = null;
        rchild = null;
    }

    public IMovie Movie
    {
        get { return movie; }
        set { movie = value; }
    }

    public BTreeNode? LChild
    {
        get { return lchild; }
        set { lchild = value; }
    }

    public BTreeNode? RChild
    {
        get { return rchild; }
        set { rchild = value; }
    }
}

// invariant: no duplicate movie in this movie collection
public class MovieCollection : IMovieCollection
{
    private BTreeNode? root; // the root of the binary search tree in which movies are stored 
    private int count; // the number of movies currently stored in this movie collection 

    public int Number { get { return count; } }

    // constructor - create an empty movie collection
    public MovieCollection()
    {
        root = null;
        count = 0;
    }

    public bool IsEmpty()
    {
        return root == null;
    }

    public bool Insert(IMovie movie)
    {
        if (movie == null)
        {
            throw new ArgumentNullException(nameof(movie), "Movie cannot be null.");
        }

        if (movie.Title == null)
        {
            throw new ArgumentNullException(nameof(movie.Title), "Movie title cannot be null.");
        }

        if (movie.TotalCopies < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(movie.TotalCopies), "Total copies cannot be negative.");
        }

        if (movie.AvailableCopies < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(movie.AvailableCopies), "Available copies cannot be negative.");
        }

        if (IsEmpty())
        {
            root = new BTreeNode(movie);
            count++;
            return true;
        }
        else if (root != null)
        {
            return Insert(root, movie);
        }
        return false;
    }

    private bool Insert(BTreeNode node, IMovie movie)
    {
        int compareResult = node.Movie.CompareTo(movie);
        if (compareResult == 0) // Movie already exists
        {
            return false;
        }
        else if (compareResult > 0) // Go left
        {
            if (node.LChild == null)
            {
                node.LChild = new BTreeNode(movie);
                count++;
                return true;
            }
            return Insert(node.LChild, movie);
        }
        else // Go right
        {
            if (node.RChild == null)
            {
                node.RChild = new BTreeNode(movie);
                count++;
                return true;
            }
            return Insert(node.RChild, movie);
        }
    }

    public bool Delete(IMovie movie)
    {
        int initialCount = count;
        root = Delete(root, movie);
        return initialCount > count;
    }

    private BTreeNode? Delete(BTreeNode? node, IMovie movie)
    {
        if (node == null)
        {
            return null;
        }

        int compareResult = node.Movie.CompareTo(movie);
        if (compareResult > 0)
        {
            node.LChild = Delete(node.LChild, movie);
        }
        else if (compareResult < 0)
        {
            node.RChild = Delete(node.RChild, movie);
        }
        else
        {
            if (node.LChild == null)
            {
                count--;
                return node.RChild;
            }
            else if (node.RChild == null)
            {
                count--;
                return node.LChild;
            }
            else
            {
                node.Movie = MinValue(node.RChild);
                node.RChild = Delete(node.RChild, node.Movie);
            }
        }
        return node;
    }

    private IMovie MinValue(BTreeNode node)
    {
        while (node.LChild != null)
        {
            node = node.LChild;
        }
        return node.Movie;
    }

    public IMovie? Search(string movietitle)
    {
        return Search(root, movietitle);
    }

    private IMovie? Search(BTreeNode? node, string movietitle)
    {
        if (node == null)
        {
            return null;
        }

        int compareResult = string.Compare(node.Movie.Title, movietitle, StringComparison.InvariantCulture);
        if (compareResult == 0)
        {
            return node.Movie;
        }
        else if (compareResult > 0)
        {
            return Search(node.LChild, movietitle);
        }
        else
        {
            return Search(node.RChild, movietitle);
        }
    }

    public int NoDVDs()
    {
        return NoDVDs(root);
    }

    private int operationCount;

    public int GetOperationCount()
    {
        return operationCount;
    }

    public void ResetOperationCount()
    {
        operationCount = 0;
    }

    private int NoDVDs(BTreeNode? node)
    {
        if (node == null)
        {
            return 0;
        }

        int count = node.Movie.AvailableCopies;
        operationCount++; // Increment the operation count for each addition operation
        count += NoDVDs(node.LChild) + NoDVDs(node.RChild);
        return count;
    }


    public IMovie[] ToArray()
    {
        IMovie[] movies = new IMovie[count];
        ToArray(root, ref movies, 0);
        return movies;
    }

    private int ToArray(BTreeNode? node, ref IMovie[] movies, int index)
    {
        if (node == null)
        {
            return index;
        }

        index = ToArray(node.LChild, ref movies, index);
        movies[index] = node.Movie;
        index++;
        return ToArray(node.RChild, ref movies, index);
    }

    public void Clear()
    {
        root = null;
        count = 0;
    }
}





