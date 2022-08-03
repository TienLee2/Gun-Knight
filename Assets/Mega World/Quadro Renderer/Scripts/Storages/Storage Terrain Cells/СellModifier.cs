using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadroRendererSystem
{
    public class СellModifier
    {
        private List<Cell> ModifiedСells = new List<Cell>();
        public bool RemoveAfterConvert = true;

        public void AddModifiedСell(Cell cell)
        {
            AddModifiedСell(cell, false, true);
        }

        public void AddModifiedСell(Cell cell, bool convertPersistentItemToObjectInstanced, bool removeAfterConvert)
        {
            if(ModifiedСells.Contains(cell) == false)
            {
                if(convertPersistentItemToObjectInstanced)
                {
                    cell.ConvertPersistentItemToObjectInstanced();
                }

                ModifiedСells.Add(cell);
                
            }

            this.RemoveAfterConvert = removeAfterConvert;
        }

        public void RemoveCells()
        {
            foreach (Cell cell in ModifiedСells)
            {
                cell.ClearObjectInstancedList();
            }

            ModifiedСells.Clear();
            RemoveAfterConvert = false;
        }

        public List<Cell> GetCells()
        {
            return ModifiedСells;
        }
    }
}