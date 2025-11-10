using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PFMG_UC
{
    static class sglobal
    {
        //public static UserControlUCBook;
    }

    interface CRUD : CRUD_UI, CRUD_APP, CRUD_DB
    {
        void OnFirst();
        MVCEntity OnNext();
        MVCEntity OnPrev();
        void OnLast();
    }

    interface CRUD_UI
    {
        void OnCreate(MVCEntity entity);
        List<MVCEntity> OnSave(MVCEntity entity);
        MVCEntity OnRead(MVCEntity entity);
        List<MVCEntity> OnUpdate(ref MVCEntity entity);
        List<MVCEntity> OnDelete(ref MVCEntity entity);
    }

    interface CRUD_APP
    {
        List<MVCEntity> Create(MVCEntity s);
        MVCEntity Read(MVCEntity s);
        List<MVCEntity> Update(ref MVCEntity s);
        List<MVCEntity> Delete(ref MVCEntity s);
    }

    interface CRUD_DB
    {
        void Flush_DB();
        void Create_DB(MVCEntity s);
        void Read_DB(MVCEntity s);
        void Update_DB(MVCEntity s);
        void Delete_DB(MVCEntity s);
        List<MVCEntity> Read_From_File(MVCEntity e);
        void Write_To_File(List<MVCEntity> entities);
    }



    public abstract class MVCEntity
    {
        //____________________________UI_Perspective_________________________________________________________
        public class Identifier
        {
            public string id;
            public string type;
        }

        public class Header
        {
            public string header;
        }
        //____________________________UI_Perspective_________________________________________________________

        public Entity_UC UC;

        //____________________________UI_Perspective_________________________________________________________


        //____________________________Application_Perspective________________________________________________

        public abstract bool Match(MVCEntity entity_list, MVCEntity entity);
        public abstract MVCEntity get_entity(string[] data, ref StreamReader reader);
        public abstract Identifier get_Identifier();


        //____________________________Application_Perspective________________________________________________


        //_________________________Persistance_Perspective___________________________________________________

        // public abstract string ToString();
        public abstract Header GetHeader(int count);
        //public abstract int get_offset();

        //_________________________Persistance_Perspective___________________________________________________
    }

    public class TOC
    {
        public MVCEntity.Identifier entity_type;
        public int offset;

        public TOC()
        {
            entity_type = new MVCEntity.Identifier();
        }
        public override string ToString()
        {
            string s = entity_type.type + "," + offset;
            return s;
        }
    }

    public class CRUDList : CRUD
    {
        public List<MVCEntity> MVCEntityList = new List<MVCEntity>();
        public MVCEntity.Header h;
        public TOC toc;

        int iterator;

        public CRUDList()
        {
            toc = new TOC();
            iterator = 0;
        }

        public CRUDList(List<MVCEntity> mvc_list)
        {
            MVCEntityList = mvc_list;
            iterator = 0;
            toc = new TOC();
        }

        public MVCEntity first() { iterator = 0; return MVCEntityList.ToArray()[iterator]; }

        public MVCEntity last() { iterator = MVCEntityList.Count - 1; return MVCEntityList.ToArray()[iterator]; }

        public MVCEntity next()
        {
            iterator++;
            if (iterator == 0)
            {
                iterator++;
            }
            return iterator < MVCEntityList.Count ? MVCEntityList.ToArray()[iterator] : null;
        }

        public MVCEntity prev()
        {
            iterator--;
            if (iterator == MVCEntityList.Count - 1)
            {
                iterator--;
            }
            return iterator > -1 ? MVCEntityList.ToArray()[iterator] : null;
        }
        //_____________________________________START________________Iterator_OnClick_Methods_________________________________________

        public void OnFirst()
        {
            MVCEntity first_rec = first();
            first_rec.UC.display(first_rec);

        }

        public MVCEntity OnNext()
        {
            MVCEntity next_rec;
            if ((next_rec = next()) == null)
                return null;
            else
                next_rec.UC.display(next_rec);
            return next_rec;
        }

        public MVCEntity OnPrev()
        {
            MVCEntity prev_rec;
            if ((prev_rec = prev()) == null)
                return null;
            else
                prev_rec.UC.display(prev_rec);
            return prev_rec;
        }

        public void OnLast()
        {
            MVCEntity last_rec = last();
            last_rec.UC.display(last_rec);
        }
        //____________________________________________END_________Iterator_OnClick_Methods_____________________________________________________________


        //_________________________________________________START_______CRUD_UI_________________________________________________________________________

        public void OnCreate(MVCEntity entity)
        {
            entity.UC.Clear_all();
            entity.UC.Enable();
        }

        public List<MVCEntity> OnSave(MVCEntity entity)
        {
            return Create(entity);
        }

        public MVCEntity OnRead(MVCEntity entity)
        {
            entity = Read(entity);
            if (entity == null)
                return null;
            return entity;
        }

        public List<MVCEntity> OnUpdate(ref MVCEntity entity)
        {
            MVCEntityList = Update(ref entity);
            if (entity == null)
                return null;
            return MVCEntityList;
        }

        public List<MVCEntity> OnDelete(ref MVCEntity entity)
        {
            MVCEntityList = Delete(ref entity);
            if (entity == null)
                return null;
            return MVCEntityList;
        }

        //_________________________________________________END_________CRUD_UI_______________________________________________________________________


        //_______________________________________________START_________CRUD_APP______________________________________________________________________

        public List<MVCEntity> Create(MVCEntity new_entry)
        {
            //Read_DB(new_entry);
            foreach (MVCEntity entity in MVCEntityList)
            {
                if (new_entry.Match(entity, new_entry))
                {
                    return null;
                }
            }
            MVCEntityList.Add(new_entry);
            return MVCEntityList;
        }

        public MVCEntity Read(MVCEntity s)
        {
            for (int i = 0; i < MVCEntityList.Count; i++)
            {
                if (s.GetType() == MVCEntityList[i].GetType())
                {
                    if (s.Match(MVCEntityList[i], s))
                    {
                        return MVCEntityList[i];
                    }
                }
            }
            return null;
        }

        public List<MVCEntity> Update(ref MVCEntity s)
        {
            for (int i = 0; i < MVCEntityList.Count; i++)
            {
                if (s.GetType() == MVCEntityList[i].GetType())
                {
                    if (s.Match(MVCEntityList[i], s))
                    {
                        MVCEntityList[i] = s;
                        Update_DB(s);
                        return MVCEntityList;
                    }
                }
            }
            return null;
        }

        public List<MVCEntity> Delete(ref MVCEntity s)
        {
            int count = MVCEntityList.Count();
            for (int i = 0; i < MVCEntityList.Count; i++)
            {
                if (s.GetType() == MVCEntityList[i].GetType())
                {
                    if (s.Match(MVCEntityList[i], s))
                    {
                        s = MVCEntityList[i];
                        MVCEntityList.Remove(MVCEntityList[i]);
                        Delete_DB(s);
                        if (MVCEntityList.Count() == count - 1) return MVCEntityList;
                        else return null;
                    }
                }
            }
            return null;
        }
        //________________________________________________END__________CRUD_APP______________________________________________________________________


        //_______________________________________________START_________CRUD_DB_______________________________________________________________________

        public void Flush_DB()
        {
            using (StreamWriter writer = new StreamWriter("Input.csv"))
            {
                writer.Flush();
                writer.Close();
            }
        }

        public void Create_DB(MVCEntity s)
        {
            using (StreamWriter writer = new StreamWriter("Input.csv", true))
            {
                writer.WriteLine(s.ToString());
                writer.Close();
            }
        }

        public void Read_DB(MVCEntity s)
        {
            MVCEntityList = Read_From_File(s);
        }

        public void Update_DB(MVCEntity s)
        {
            //Write_To_File(MVCEntityList);
        }

        public void Delete_DB(MVCEntity s)
        {
            // Write_To_File(MVCEntityList);
        }
        //_______________________________________________END__________CRUD_DB_______________________________________________________________________

        public void Write_To_File(List<MVCEntity> entities)
        {

            using (StreamWriter writer = new StreamWriter("Input.csv", true))
            {
                string s;
                h = entities[0].GetHeader(entities.Count());
                writer.WriteLine(h.header);

                foreach (MVCEntity st in entities)
                {
                    s = st.ToString();
                    writer.WriteLine(s);
                }
                writer.WriteLine("#" + "," + "#" + "," + "#" + "," + "#");
                writer.Close();
            }
        }

        public List<MVCEntity> Read_From_File(MVCEntity e)
        {
            List<MVCEntity> entities = new List<MVCEntity>();
            StreamReader reader = new StreamReader("Input.csv", true);

            string S = reader.ReadLine();
            while (S != null)
            {
                string[] data = S.Split(',');
                MVCEntity.Identifier identifier = e.get_Identifier();

                if (data[0] == identifier.id)
                {
                    MVCEntity c = e.get_entity(data, ref reader);
                    entities.Add(c);

                    if (entities.Count() > 1)
                    {
                        for (int i = 0; i < entities.Count() - 1; i++)
                        {
                            if (e.Match(entities[i], c))
                            {
                                entities.Remove(c);
                                break;
                            }
                        }
                    }
                }
                S = reader.ReadLine();
            }
            return entities;
        }

        public TOC Get_TOC(MVCEntity e)
        {
            using (StreamReader reader = new StreamReader("Input.csv", true))
            {
                int count = 0;
                string S = reader.ReadLine();
                while (S != null)
                {
                    count++;
                    string[] data = S.Split(',');
                    MVCEntity.Identifier identifier = e.get_Identifier();
                    if (data[0] == identifier.id)
                    {
                        toc.entity_type = e.get_Identifier();
                        toc.offset = count;
                        return toc;
                    }
                    S = reader.ReadLine();
                }
                return toc;
            }
        }
    }
}
