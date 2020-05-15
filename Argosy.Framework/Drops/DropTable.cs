using System;
using System.Linq;
using System.Collections.Generic;

namespace Argosy.Framework {
    // TODO: create DropTableWithoutReplacement with overrided GenerateDrops method
    public class DropTable : DroppableGeneric<DropTable>, ICustomDropInfo {
        public string UniqueID { get; private set; }
        public uint DropAmount { get; private set; }

        private readonly Random random = new Random();

        // Use hash sets to prevent dupes because there is no reason to have duplicate elements in a drop table
        private readonly HashSet<IDroppable> contents;
        private readonly List<IDroppable> generatedDropList;
        private readonly List<IDroppable> weightedDropList;

        #region CONSTRUCTORS
        public DropTable(string uniqueID)                                                      : this(uniqueID, 0) { }
        public DropTable(string uniqueID, uint dropAmount)                                     : this(uniqueID, dropAmount, 0d) { }
        public DropTable(string uniqueID, uint dropAmount, double weight)                      : this(uniqueID, dropAmount, weight, false) { }
        public DropTable(string uniqueID, uint dropAmount, double weight, bool guaranteedDrop) : base(weight, guaranteedDrop) {
            UniqueID = uniqueID;
            DropAmount = dropAmount;

            SetCustomDropInfo(this);

            contents = new HashSet<IDroppable>();
            generatedDropList = new List<IDroppable>((int)dropAmount);
            weightedDropList = new List<IDroppable>((int)dropAmount);
        }
        #endregion

        #region PRIVATE METHODS
        private double getTotalWeight(IEnumerable<IDroppable> list) {
            double totalWeight = 0d;

            foreach (var drop in list) {
                totalWeight += drop.Weight;
            }

            return totalWeight;
        }
        #endregion

        public List<IDroppable> GetContentsAsList() {
            return contents.ToList();
        }

        public virtual void Add(IDroppable entry) {
            if (entry == null) {
                throw new ArgumentNullException();
            }

            this.Add(entry, entry.Weight, entry.GuaranteedDrop);
        }

        public virtual void Add(IDroppable entry, double weight) {
            if (entry == null) {
                throw new ArgumentNullException();
            }

            this.Add(entry, weight, entry.GuaranteedDrop);
        }

        public virtual void Add(IDroppable entry, double weight, bool guaranteedDrop) {
            if (entry == null) {
                throw new ArgumentNullException();
            }

            entry.Weight = weight;
            entry.GuaranteedDrop = guaranteedDrop;

            // TODO: put in another check for NullDrops because it is the only exception where adding a droppable can exceed the drop amount
            if (contents.Count != 0 && contents.Count + 1 > DropAmount) {
                throw new Exception("Attempting to add a drop in excess of the drop table's drop amount");
            }

            if (!contents.Add(entry)) {
                throw new Exception(entry + " already exists in the drop table");
            }
        }

        public virtual void Reset() {
            contents.Clear();
        }

        public virtual IEnumerable<IDroppable> GenerateDrops() {
            generatedDropList.Clear();
            weightedDropList.Clear();

            foreach (IDroppable drop in contents.Where(x => !x.GuaranteedDrop)) {
                weightedDropList.Add(drop);
            }

            foreach (IDroppable drop in contents.Where(x => x.GuaranteedDrop)) {
                generatedDropList.Add(drop);
            }

            // Calculate the adjusted amount after factoring in guaranteed drops
            int newDropAmount = (int)DropAmount - generatedDropList.Count;

            double totalWeight = getTotalWeight(weightedDropList);
            double randomNumber;
            double cumulativeSum;

            for (int i = 0; i < newDropAmount; i++) {
                // System.Random.NextDouble returns 0.0 inclusive to 1.0 exclusive 
                // (i.e. the upper bound will be fractionally less than totalWeight)
                randomNumber = random.NextDouble() * totalWeight;
                cumulativeSum = 0d;

                // To function as intended, the list must be sorted by ascending weights before iterating it
                foreach (IDroppable drop in weightedDropList.OrderBy(x => x.Weight)) {
                    cumulativeSum += drop.Weight;

                    // Use < instead of <= so that elements with 0 weight have absolutely no chance of dropping
                    // Using <= also skews the drop chances by a measurable amount, which makes it imprecise
                    // As evidence, chart out a table of 3 elements with weights 1, 2, 3 (totalWeight 6)
                    if (randomNumber < cumulativeSum) {
                        generatedDropList.Add(drop);
                        break;
                    }
                }
            }

            return generatedDropList;
        }
    }
}