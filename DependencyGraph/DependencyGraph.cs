// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates. If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    ///
    /// Given a DependencyGraph DG:
    ///
    /// (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    /// (The set of things that depend on s)
    ///
    /// (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    /// (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    // dependents("a") = {"b", "c"}
    // dependents("b") = {"d"}
    // dependents("c") = {}
    // dependents("d") = {"d"}
    // dependees("a") = {}
    // dependees("b") = {"a"}
    // dependees("c") = {"a"}
    // dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //Declare 2 private dictionary with key string and value HashSet<string>
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();  
    }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                int size = 0;
                foreach (var pair in dependents) { size += pair.Value.Count; }
                return size;
            }
        }
        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer. If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                return dependents[s].Count;
            }
        }
        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.ContainsKey(s);
        }
        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.ContainsKey(s);
        }
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependees.ContainsKey(s))
            {
                 return dependees[s];
            }
            else { return Enumerable.Empty<string>(); }
        }
        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (dependents.ContainsKey(s))
            {
                return dependents[s];
            }
            else { return Enumerable.Empty<string>(); }    
        }
        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///
        /// <para>This should be thought of as:</para>
        ///
        /// t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param> ///
        public void AddDependency(string s, string t)
        {
            //check for empty node in dependents
            if (!dependents.ContainsKey(t))
            {
                //create a new node
                dependents[t] = new HashSet<string>();
            }
            //check for empty node in dependees
            if (!dependees.ContainsKey(s))
            {
                //create a new node
                dependees[s] = new HashSet<string>();
            }
            //add value t into dependent s
            dependents[t].Add(s);
            //add value s in dependees t
            dependees[s].Add(t);
        }
        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //check if they exist
            if (dependents.ContainsKey(t) && dependees.ContainsKey(s))
            {
                //remove each one
                dependents[t].Remove(s);
                dependees[s].Remove(t);
                //check if s not exist in dependents => remove
                if (dependents[t].Count == 0) { dependents.Remove(t); }
                //check if t not exist in dependees => remove
                if (dependees[s].Count == 0) { dependees.Remove(s); }
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //check to see if s exist in dictionary dependents
            if (dependees.ContainsKey(s)) 
            { 
                //remove all s from dependent
                foreach (var existDependees in dependees[s].ToList()) 
                { 
                    if (newDependents.Count() == 0)
                    {
                        RemoveDependency(s, existDependees);
                        AddDependency(s,"");
                    }
                    foreach (var newDependees in newDependents)
                    {
                        {
                            RemoveDependency(s, existDependees);
                            AddDependency(s, newDependees);
                        }
                    }
                }
            }
            
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //check to see if s exist in dictionary dependees
            if (dependents.ContainsKey(s)) 
            {
                //remove all s from dependees
                foreach (var existDependents in dependents[s].ToList()) 
                {
                    if (newDependees.Count() == 0)
                    {
                        RemoveDependency(existDependents, s);
                        AddDependency("", s);
                    }
                    foreach (var newDependents in newDependees)
                    {
                        RemoveDependency(existDependents, s);
                        AddDependency(newDependents, s);
                    }
                }
                //add in the new dependees
                
            }
        }
    }
}