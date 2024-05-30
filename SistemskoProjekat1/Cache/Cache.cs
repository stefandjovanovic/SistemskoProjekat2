using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SistemskoProjekat2.Cache
{
    public class Cache
    {

        //Implementacija kesa pomocu lancane liste po principu LRU

        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();


        public Dictionary<string, Node> Entries;
        public int Capacity { get; set; }
        public Node? Head { get; set; }
        public Node? Tail { get; set; }
        public int Length { get; set; }

        public Cache(int Capacity)
        {
            this.Capacity = Capacity;
            this.Length = 0;
            Entries = new Dictionary<string, Node>();
        }

        public void WriteToCache(string key, string value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (Entries.ContainsKey(key))
                {
                    //Ako postoji elment sa datim kljucem postavi ga na head
                    Node n = Entries[key];
                    n.Data = value;

                    Remove(ref n);
                    SetHead(ref n);
                }
                else //Ako ne postoji element kreiraj i dodaj na head
                {
                    if (Length < Capacity)
                    {
                        Node n = new Node(key, value);

                        SetHead(ref n);

                        Length++;

                        Entries.Add(key, n);
                    }
                    else //Kes je pun izbrisi sa poslednji elemnt i dodaj novi
                    {
                        Entries.Remove(Tail!.Key);
                        Node p1 = Tail;
                        Remove(ref p1);

                        Node n = new Node(key, value);
                        SetHead(ref n);

                        Entries.Add(key, n);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
                throw;
            }
            finally {
                cacheLock.ExitWriteLock(); 
            }
            


        }

        public string? ReadCache(string key)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (Entries.ContainsKey(key))
                {
                    Node n = Entries[key];
                    //Ako nije head smesti ga
                    if (!IsHead(n))
                    {
                        Remove(ref n);

                        SetHead(ref n);
                    }

                    return n.Data;
                }
                else
                {
                    //Ako ne postoji vraca null
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitReadLock();

            }
            
        }

        public void SetHead(ref Node n)
        {
            if (Head == null)
            {
                Head = n;
                Tail = n;
            }
            else
            {
                Head.Prev = n;
                n.Next = Head;
                Head = n;
            }
        }

        public void Remove(ref Node n)
        {
            Node? pre = n.Prev;
            Node? post = n.Next;

            if (pre == null)
            {
                Head = post;
            }
            else
            {
                pre.Next = post;
            }

            if (post == null)
            {
                Tail = pre;
            }
            else
            {
                post.Prev = pre;
            }
        }
        private bool IsHead(Node n) {
            if (n != null && Head != null && n.Key == Head.Key) 
                return true;
            return false;
        }
    }
}
