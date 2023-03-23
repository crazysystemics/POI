using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PFMG
{
    interface CRUD : CRUD_UI, CRUD_APP, CRUD_DB
    {

    }

    interface CRUD_UI
    {
        void OnCreate(MVCEntity e);
        MVCEntity OnRead(MVCEntity e);
        MVCEntity OnUpdate(MVCEntity e);
        MVCEntity OnDelete(MVCEntity e);
    }

    interface CRUD_APP
    {
        void Create(MVCEntity e);
        MVCEntity Read(MVCEntity e);
        MVCEntity Update(MVCEntity e);
        MVCEntity Delete(MVCEntity e);
    }
    interface CRUD_DB
    {
        void CreateDB(MVCEntity e);
        void ReadDB();
        void UpdateDB(MVCEntity e);
        void DeleteDB();
        void Write_To_File(List<MVCEntity> mvc_list);
        List<MVCEntity> Read_From_File(MVCEntity e);
    }

    public abstract class MVCEntity
    {
        public Entity_UC UC;
        public abstract bool Match(MVCEntity list_entity, MVCEntity e);
        public abstract MVCEntity get_input_file_entity(string[] data);
    }

    public class CRUDList : CRUD
    {
        public List<MVCEntity> MVCEntity_List;

        public CRUDList()
        {

        }
        public CRUDList(List<MVCEntity> pmvcentity_list)
        {
            MVCEntity_List = pmvcentity_list;
        }

        public void OnCreate(MVCEntity e)
        {
            Create(e);
        }
        public MVCEntity OnRead(MVCEntity e) 
        {
            return Read(e);
        }
        public MVCEntity OnUpdate(MVCEntity e) 
        {
            return Update(e);
        
        }
        public MVCEntity OnDelete(MVCEntity e) 
        {
            return Delete(e);
        }

        public void Create(MVCEntity e) 
        {
            MVCEntity_List.Add(e);
            CreateDB(e);
        }
        public MVCEntity Read(MVCEntity e) 
        {
            foreach(MVCEntity entity in MVCEntity_List)
            {
                if (e.Match(entity , e))
                {
                    return entity;
                }
            }
            return null;
        }        
        public MVCEntity Update(MVCEntity e) 
        {
            //foreach (MVCEntity entity in MVCEntity_List)
            for(int i = 0; i < MVCEntity_List.Count; i++)
            {
                if (e.Match(MVCEntity_List[i], e))
                {
                    MVCEntity_List[i] = e;
                    return e;
                }
            }
            return null;
        }
        public MVCEntity Delete(MVCEntity e) 
        {
            foreach (MVCEntity entity in MVCEntity_List)
            {
                if (e.Match(entity, e))
                {
                    MVCEntity_List.Remove(entity);
                    return e;
                }
            }
            return null;
        }

        public void CreateDB(MVCEntity e) 
        {
            using(StreamWriter writer = new StreamWriter("DataBase.csv", true))
            {
                writer.WriteLine(e.ToString());
            }
        }
        public void ReadDB() { }
        public void UpdateDB(MVCEntity e) 
        { 
            
        }
        public void DeleteDB() 
        { 

        }
        
        public void Write_To_File(List<MVCEntity> mvc_list)
        {
            using (StreamWriter writer = new StreamWriter("DataBase.csv", true))
            {
                foreach(MVCEntity entity in mvc_list)
                {
                    writer.WriteLine(entity.ToString());
                }
                writer.Close();
            }
        }
        public List<MVCEntity> Read_From_File(MVCEntity e)
        {
            List<MVCEntity> mvclist = new List<MVCEntity>();
            StreamReader reader = new StreamReader("DataBase.csv", true);
            string S = reader.ReadLine();
            while(S != null)
            {
                string[] data = S.Split(",");
                mvclist.Add(e.get_input_file_entity(data));
                S = reader.ReadLine();
            }
            
            return mvclist;
        }

        public void OnFlush_DB()
        {
            using (StreamWriter writer = new StreamWriter("DataBase.csv"))
            {
                writer.Flush();
                writer.Close();
            }
        }
    }
}
