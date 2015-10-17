using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;





public partial class currentUser {

    private int _id;
    public int USER_ID {
        set {
            _id = value;
        }
        get {
            return _id;
        }
    }
    private string _email;
    public string EMAIL {
        set {
            _email = value;
        }
        get {
            return _email;
        }
    }
    private string _userName;
    public string USERNAME {
        set {
            _userName = value;
        }
        get {
            return _userName;
        }
    }
    private DateTime _timeIn;
    public DateTime TIMEIN {
        set {
            _timeIn = value;
        }
        get {
            return _timeIn;
        }
    }
    private bool _isMOEAdmin;
    public bool isMOEAdmin {
        set {
            _isMOEAdmin = value;
        }
        get {
            return _isMOEAdmin;
        }
    }
    private bool _isEO;
    public bool isEO {
        set {
            _isEO = value;
        }
        get {
            return _isEO;
        }
    }
    private bool _isTeacher;
    public bool isTeacher {
        set {
            _isTeacher = value;
        }
        get {
            return _isTeacher;
        }
    }
    private bool _isSchoolAdmin;
    public bool isSchoolAdmin {
        set {
            _isSchoolAdmin = value;
        }
        get {
            return _isSchoolAdmin;
        }
    }

    public currentUser() {
    }//consrtutor        
}//class







/// <summary>
/// Summary description for MOE_WebService
/// </summary>
[WebService( Namespace = "http:/MOE.EDU.GOV.JM/" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class MOE_WebService : System.Web.Services.WebService {

    //to get access to the data class model aka database
    MOE_DataClassModelDataContext  MOE_DB = new MOE_DataClassModelDataContext();


    //for user log on and access privileges 
    currentUser CurrentUSER = new currentUser();



    public MOE_WebService() {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //test method
    [WebMethod]
    public string HelloWorld() {
        return "Hello World   -" + "lets go!!";
    }












    /******************************************************
     *                  crypto methods
     *
     * ****************************************************/

    //to encrypt the password before storing it in the DB
    private string Encrypt( string clearText ) {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes( clearText );
        using (Aes encryptor = Aes.Create()) {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 
                                                                                       0x6e, 0x20, 0x4d, 0x65, 0x64, 
                                                                                       0x76, 0x65, 0x64, 0x65, 0x76 } );
            encryptor.Key = pdb.GetBytes( 32 );
            encryptor.IV = pdb.GetBytes( 16 );
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream( ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write )) {
                    cs.Write( clearBytes, 0, clearBytes.Length );
                    cs.Close();
                }
                clearText = Convert.ToBase64String( ms.ToArray() );
            }
        }
        return clearText;
    }//encrypt


    //to decrypt the password 
    private string Decrypt( string cipherText ) {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] cipherBytes = Convert.FromBase64String( cipherText );
        using (Aes encryptor = Aes.Create()) {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes( EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 
                                                                                        0x6e, 0x20, 0x4d, 0x65, 0x64, 
                                                                                        0x76, 0x65, 0x64, 0x65, 0x76 } );
            encryptor.Key = pdb.GetBytes( 32 );
            encryptor.IV = pdb.GetBytes( 16 );
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream( ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write )) {
                    cs.Write( cipherBytes, 0, cipherBytes.Length );
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString( ms.ToArray() );
            }
        }
        return cipherText;
    }//decrypt




    /******************************************************
     *                   Teacher Methods
     *
     * ****************************************************/

    [WebMethod]
    public bool teacherExists( string fname, string mid, string lname, DateTime dob ) {
        bool exists = false;
        try {
            teacher checking = (from t in MOE_DB.teachers
                                where t.firstName == fname && t.middleName == mid && t.lastName == lname && t.DOB == dob
                                select t).Single();

            if (checking != null)
                exists = true;

        }
        catch (Exception ex) {
            exists = false;
        }

        return exists;
    }//teacherExist



    [WebMethod]
    public teacher getTeacher( string fname, string mid, string lname, DateTime dob ) {

        try {
            teacher checking = (from t in MOE_DB.teachers
                                where t.firstName == fname && t.middleName == mid && t.lastName == lname && t.DOB == dob
                                select t).Single();

            if (checking != null)
                return checking;

        }
        catch (Exception ex) {
            return null;
        }

        return null;
    }//get teacher


    [WebMethod]
    public teacher getTeacherByID( int id ) {

        try {
            teacher checking = (from t in MOE_DB.teachers
                                where t.teacher_ID == id
                                select t).Single();

            if (checking != null)
                return checking;

        }
        catch (Exception ex) {
            return null;
        }

        return null;
    }


    [WebMethod]
    public teacher getTeacherByUserID( int u_id ) {

        try {
            teacher checking = (from t in MOE_DB.teachers
                                where t.user__ID == u_id
                                select t).Single();

            if (checking != null)
                return checking;

        }
        catch (Exception ex) {
            return null;
        }

        return null;
    }

    [WebMethod]
    public teacher[] getAllTeachers() {
        try {
            teacher[] teachers = (from t in MOE_DB.teachers
                                  select t).ToArray();

            if (teachers != null)
                return teachers;
            else
                return null;


        }
        catch (Exception ex) {
            return null;
        }

    }//get all teachers


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isTeacherAdded( string fname, string mid, string lname, DateTime dob, string position, int u_ID ) {

        if (!teacherExists( fname, mid, lname, dob )) {
            try {
                teacher newTeacher = new teacher() {

                    firstName = fname,
                    middleName = mid,
                    lastName = lname,
                    DOB = dob,
                    position = position,
                    user__ID = u_ID
                };

                MOE_DB.teachers.InsertOnSubmit( newTeacher );
                MOE_DB.SubmitChanges();

                return true;

            }
            catch (Exception ex) {
                return false;
            }//try catch
        }//if
        return false;
    }//isStudentAdded




    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isTeacherUpdated( string fname, string mid, string lname, DateTime dob, string position, int u_ID ) {

        if (!teacherExists( fname, mid, lname, dob )) {
            try {

                teacher oldTeacher = getTeacher( fname, mid, lname, dob );
                teacher newTeacher = new teacher() {

                    firstName = fname,
                    middleName = mid,
                    lastName = lname,
                    DOB = dob,
                    position = position,
                    user__ID = u_ID
                };

                int id = oldTeacher.teacher_ID;
                oldTeacher = newTeacher;
                oldTeacher.teacher_ID = id;


                MOE_DB.SubmitChanges();

                return true;

            }
            catch (Exception ex) {
                return false;
            }//try catch
        }//if
        return false;
    }//isStudentAdded



    [WebMethod]
    public bool isTeacherDeleted( int ID ) {
        try {
            teacher byebye = getTeacherByID( ID );

            MOE_DB.teachers.DeleteOnSubmit( byebye );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }//try/catch
    }//isstudentdeleted


    [WebMethod]
    public teacher[] getTeachersBySubject( string subject ) {
        try {
            teacher[] teachers = (from t in MOE_DB.teachers
                                  join st in MOE_DB.subjectTeachers
                                  on t.teacher_ID equals st.teacher_ID
                                  join s in MOE_DB.subjects
                                  on st.subject_ID equals s.subject_ID
                                  where s.name == subject
                                  select t).ToArray();
            if (teachers != null)
                return teachers;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//trycatch
    }//getTeachersBySubject


    [WebMethod]
    public teacher getFormclassTeacher( int formClassID ) {
        teacher teach = (from t in MOE_DB.teachers
                         join classT in MOE_DB.formclasses
                             on t.teacher_ID equals classT.teacher_ID
                         where classT.formclass_ID == formClassID
                         select t).Single();
        if (teach != null)
            return teach;
        else
            return null;
    }//getFormclassTeacher


    [WebMethod]
    public formclass getTeacherFormclass( int teacherID ) {
        formclass FormC = (from f in MOE_DB.formclasses
                           where f.teacher_ID == teacherID
                           select f).Single();
        if (FormC != null)
            return FormC;
        else
            return null;
    }//getTeacherFormclass




    /******************************************************
   *                  Education Officer methods
   *
   * ****************************************************/


    [WebMethod]
    public edu_officer getEduOfficerByUserID( int u_id ) {
        try {
            edu_officer thisOne = (from eo in MOE_DB.edu_officers
                                   where eo.user_ID == u_id
                                   select eo).Single();
            if (thisOne != null)
                return thisOne;
            else
                return null;

        }
        catch (Exception ex) {
            return null;
        }

    }//getEduOfficerByUserID

    [WebMethod]
    public bool EduOfficerExists( string fName, string mName, string lName, DateTime dob ) {
        try {
            edu_officer eo = (from e in MOE_DB.edu_officers
                              where e.firstName == fName && e.lastName == lName && e.DOB == dob && e.middleName == mName
                              select e).Single();
            if (eo != null)
                return true;
            else
                return false;


        }
        catch (Exception ex) {
            return false;
        }//try catch
    }//EduOfficerExists


    [WebMethod]
    public edu_officer getEduOfficer( string fName, string mName, string lName, DateTime dob ) {
        try {

            edu_officer EO = (from e in MOE_DB.edu_officers
                              where e.firstName == fName && e.lastName == lName && e.DOB == dob && e.middleName == mName
                              select e).Single();
            if (EO != null)
                return EO;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//tryCatch

    }//getEduOfficer



    [WebMethod]
    public edu_officer getEduOfficerByID( int id ) {
        try {

            edu_officer EO = (from e in MOE_DB.edu_officers
                              where e.edu_officer_ID == id
                              select e).Single();
            if (EO != null)
                return EO;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//tryCatch

    }//getEduOfficerByID

    [WebMethod]
    public edu_officer[] getAllEduOfficers() {
        try {
            edu_officer[] EOs = (from e in MOE_DB.edu_officers
                                 select e).ToArray();
            if (EOs != null)
                return EOs;
            else
                return null;

        }
        catch (Exception ex) {
            return null;
        }
    }//getAllEduOfficers

    [WebMethod]
    public bool isEduOfficerAdded( string fname, string mname, string lname, DateTime dob, string position, int userID ) {
        if (!EduOfficerExists( fname, mname, lname, dob )) {
            edu_officer newEO = new edu_officer() {
                firstName = fname,
                lastName = lname,
                middleName = mname,
                DOB = dob,
                position = position,
                user_ID = userID
            };

            try {
                MOE_DB.edu_officers.InsertOnSubmit( newEO );
                MOE_DB.SubmitChanges();
                return true;

            }
            catch (Exception ex) {
                return false;
            }
        }
        else
            return false;
    }//isEduOfficerAdded



    [WebMethod]
    public bool isEduOfficerUpdated( string fname, string mname, string lname, DateTime dob, string position, int userID ) {
        if (!EduOfficerExists( fname, mname, lname, dob )) {
            edu_officer oldEO = getEduOfficer( fname, mname, lname, dob );

            edu_officer newEO = new edu_officer() {
                firstName = fname,
                lastName = lname,
                middleName = mname,
                DOB = dob,
                position = position,
                user_ID = userID
            };

            oldEO = newEO;

            try {
                MOE_DB.SubmitChanges();
                return true;

            }
            catch (Exception ex) {
                return false;
            }
        }
        else
            return false;
    }//isEduOfficerUpdated


    [WebMethod]
    public bool isEduOfficerDeleted( int id ) {
        edu_officer byebye = getEduOfficerByID( id );
        try {
            MOE_DB.edu_officers.DeleteOnSubmit( byebye );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isEduOfficerDeleted



    [WebMethod]
    public edu_officer[] getEduOfficersBySubject( string subject ) {
        edu_officer[] eos = (from e in MOE_DB.edu_officers
                             join es in MOE_DB.edu_officerSubjects
                                 on e.user_ID equals es.edu_officer_ID
                             join s in MOE_DB.subjects
                             on es.subject_ID equals s.subject_ID
                             where s.name == subject
                             select e).ToArray();

        if (eos != null)
            return eos;
        else
            return null;

    }//getEduOfficersBySubject




    [WebMethod]
    public edu_officer[] getEduOfficersBySubjectID( int id ) {
        edu_officer[] eo = (from e in MOE_DB.edu_officers
                            join es in MOE_DB.edu_officerSubjects
                                on e.user_ID equals es.edu_officer_ID
                            join s in MOE_DB.subjects
                            on es.subject_ID equals s.subject_ID
                            where s.subject_ID == id
                            select e).ToArray();
        if (eo != null)
            return eo;
        else
            return null;

    }//getEduOfficersBySubjectID


    [WebMethod]
    public edu_officer[] getEduOfficerByPosition( string position ) {
        edu_officer[] eos = (from e in MOE_DB.edu_officers
                             where e.position == position
                             select e).ToArray();
        if (eos != null)
            return eos;
        else
            return null;
    }//getEduOfficerByPosition


    [WebMethod]
    public bool isEduOfficerSubjectAdded( int subjectID, int edu_officerID ) {
        edu_officerSubject eos = new edu_officerSubject() {
            subject_ID = subjectID,
            edu_officer_ID = edu_officerID
        };
        try {
            MOE_DB.edu_officerSubjects.InsertOnSubmit( eos );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isEduOfficerSubjectAdded


    [WebMethod]
    public bool isEduOfficerCommentAdded( int EduOfficerID, int LessonplanID, string comment, DateTime dateofcomment ) {
        edu_officerComment eduOComment = new edu_officerComment() {
            edu_officer_ID = EduOfficerID,
            lessonPlan_ID = LessonplanID,
            Comment = comment,
            dateOfComment = dateofcomment
        };

        try {
            MOE_DB.edu_officerComments.InsertOnSubmit( eduOComment );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isEduOfficerCommentAdded



    [WebMethod]
    public bool isEduOfficerCommentUpdated( int EduOfficerID, int LessonplanID, string comment, DateTime dateofcomment ) {
        edu_officerComment eduOComment = new edu_officerComment() {
            edu_officer_ID = EduOfficerID,
            lessonPlan_ID = LessonplanID,
            Comment = comment,
            dateOfComment = dateofcomment
        };

        edu_officerComment oldComment = getEduOfficerComment(EduOfficerID, LessonplanID);

        try {
            MOE_DB.edu_officerComments.InsertOnSubmit( eduOComment );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isEduOfficerCommentUpdated


    [WebMethod]
    public bool isEduOfficerCommentDeleted(int eduofficerID, int lessonplanID) {
        edu_officerComment byebye = getEduOfficerComment(eduofficerID, lessonplanID);

        try {
            MOE_DB.edu_officerComments.DeleteOnSubmit(byebye);
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
     }//iseduOfficerCommentDeleted


    [WebMethod]
    public edu_officerComment getEduOfficerComment(int eduofficerID, int lessonplanID) {
       edu_officerComment eduOcomment = (from ec in MOE_DB.edu_officerComments
                                            where ec.edu_officer_ID == eduofficerID && ec.lessonPlan_ID == lessonplanID
                                            select ec).Single();
       if (eduOcomment != null)
           return eduOcomment;
       else
           return null;
    }//getEduOfficerComment


    [WebMethod]
    public edu_officerComment[] getAllEduOfficerComments( int eduofficerID ) {
        edu_officerComment[] eduOcomments = (from ec in MOE_DB.edu_officerComments
                                             where ec.edu_officer_ID == eduofficerID
                                             select ec).ToArray();
        if (eduOcomments != null)
            return eduOcomments;
        else
            return null;
    }//getAllEduOfficerComments




    /******************************************************
     *                   Subject Methods
     *
     * ****************************************************/
    [WebMethod]
    public bool subjectExists( string SubjecT ) {
        subject s = (from sub in MOE_DB.subjects
                     where sub.name == SubjecT
                     select sub).Single();
        if (s != null)
            return true;
        else
            return false;

    }//subjectExists

    [WebMethod]
    public bool isSubjectAdded( string name, string description ) {

        if (!subjectExists( name )) {
            try {
                subject newSubject = new subject() {
                    name = name,
                    description = description
                };
                MOE_DB.subjects.InsertOnSubmit( newSubject );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;

    }//isSubjectAdded

    [WebMethod]
    public bool isSubjectTeachersAdded( int subjectID, int teacherID ) {
        subjectTeacher st = new subjectTeacher() {
            subject_ID = subjectID,
            teacher_ID = teacherID
        };
        try {
            MOE_DB.subjectTeachers.InsertOnSubmit( st );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isSubjectTeacherAdded

    [WebMethod]
    public bool isSubjectUpdated( int id, string name, string description ) {
        subject oldSubject = (from s in MOE_DB.subjects
                              where s.subject_ID == id
                              select s).Single();
        oldSubject.name = name;
        oldSubject.description = description;
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }//try catch
    }//isSubjectUpdated

    [WebMethod]
    public bool isSubjectDeleted( int id ) {
        subject byebye = getSubjectByID( id );
        try {
            MOE_DB.subjects.DeleteOnSubmit( byebye );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isSubjectDeleted

    [WebMethod]
    public subject getSubjectByID( int id ) {
        subject sub = (from s in MOE_DB.subjects
                       where s.subject_ID == id
                       select s).Single();
        if (sub != null)
            return sub;
        else
            return null;
    }//getSubjectByID

    [WebMethod]
    public subject[] getAllSubjects() {
        subject[] subs = (from s in MOE_DB.subjects
                          select s).ToArray();
        if (subs != null)
            return subs;
        else
            return null;
    }//getAllSubjects

    [WebMethod]
    public teacher[] getSubjectTeachers( int subjectID ) {
        teacher[] subTeachers = (from t in MOE_DB.teachers
                                 join st in MOE_DB.subjectTeachers
                                     on t.teacher_ID equals st.teacher_ID
                                 join s in MOE_DB.subjects
                                     on st.subject_ID equals s.subject_ID
                                 where s.subject_ID == subjectID
                                 select t).ToArray();
        if (subTeachers != null)
            return subTeachers;
        else
            return null;
    }//getSubjectTeachers

    [WebMethod]
    public subject[] getTeacherSubjects( int teacherID ) {
        subject[] tSubs = (from s in MOE_DB.subjects
                           join st in MOE_DB.subjectTeachers
                               on s.subject_ID equals st.subject_ID
                           join t in MOE_DB.teachers
                               on st.teacher_ID equals t.teacher_ID
                           where t.teacher_ID == teacherID
                           select s).ToArray();
        if (tSubs != null)
            return tSubs;
        else
            return null;
    }//getTeacherSubjects




    /******************************************************
     *                   user Methods
     *
     * ****************************************************/
    [WebMethod]
    public bool logMeIn( string email, string pass ) {
        try {
            user fromDB = getUserByEmail( email );
            if (fromDB == null)
                return false;
            else
                if (checkPassword( pass, fromDB.password )) {//check password

                    teacher thisTeacher = getTeacherByUserID( fromDB.user_ID );
                    if (thisTeacher != null) {

                        CurrentUSER.isSchoolAdmin = Convert.ToBoolean( fromDB.isSchoolAdmin );
                        CurrentUSER.isTeacher = true;

                    }//if

                    edu_officer thisEO = getEduOfficerByID( fromDB.user_ID );
                    if (thisEO != null) {
                        CurrentUSER.isSchoolAdmin = false;
                        CurrentUSER.isEO = true;
                        CurrentUSER.isMOEAdmin = Convert.ToBoolean( fromDB.isMinistryAdmin );

                    }//if

                    CurrentUSER.EMAIL = email;
                    CurrentUSER.USER_ID = fromDB.user_ID;
                    CurrentUSER.USERNAME = fromDB.userName;
                    CurrentUSER.TIMEIN = DateTime.Now;

                    return true;

                }//if check password
                else {
                    return false;
                }//else
        }//try
        catch (Exception ex) {
            return false;
        }//catch

        return false;
    }//logmein



    [WebMethod]
    public bool userExistsByEmail( string email ) {
        try {
            user userrr = (from u in MOE_DB.users
                           where u.email == email
                           select u).Single();

            if (userrr != null)
                return true;
            else
                return false;

        }
        catch (Exception ex) {
            return false;
        }
    }//userExistsbyemail


    [WebMethod( Description = "Description: "
                          + "Arguments:  "
                          + "precondition: "
                          + "postcondition: "
                          + "Example: " )]
    public bool isPasswordChanged( string email, string oldPass, string newPass1, string newPass2 ) {
        user thisSucker;
        if (userExistsByEmail( email )) {
            thisSucker = getUserByEmail( email );
        }
        else
            return false;


        if (newPass1 != newPass2)
            return false;
        else
            if (checkPassword( oldPass, thisSucker.password )) {
                thisSucker.password = Encrypt( newPass1 );
                MOE_DB.SubmitChanges();
                return true;
            }
        return false;
    }//isPasswordChanged



    private bool checkPassword( string pass, string fromDB ) {
        if (Decrypt( fromDB ) == pass)
            return true;

        return false;
    }//check password



    [WebMethod]
    public bool userExists( int id ) {
        try {
            user userrr = (from u in MOE_DB.users
                           where u.user_ID == id
                           select u).Single();

            if (userrr != null)
                return true;
            else
                return false;

        }
        catch (Exception ex) {
            return false;
        }
    }


    [WebMethod]
    public bool isUserAdded( string user_name, string email, string pass, bool isSchAdmin, bool isMinAdmin ) {
        if (!userExistsByEmail( email )) {
            try {
                user newUser = new user() {
                    userName = user_name,
                    email = email,
                    password = Encrypt( pass ),
                    isMinistryAdmin = isMinAdmin,
                    isSchoolAdmin = isSchAdmin
                };
                MOE_DB.users.InsertOnSubmit( newUser );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }//if
        return true;
    }//isuseradded



    [WebMethod( Description = "Description: "
                          + "Arguments:  "
                          + "precondition: "
                          + "postcondition: "
                          + "Example: " )]
    public bool isUserUpdated( int id, string user_name, string ema, string pass, bool isSchAdmin, bool isMinAdmin ) {
        if (userExists( id )) {
            try {
                user newUser = new user() {
                    userName = user_name,
                    email = ema,
                    password = Encrypt( pass ),
                    isMinistryAdmin = isMinAdmin,
                    isSchoolAdmin = isSchAdmin
                };
                user oldUser = (from u in MOE_DB.users
                                where u.user_ID == id
                                select u).Single();
                oldUser = newUser;
                oldUser.user_ID = id;


                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }//if
        return true;
    }//isuserupdated



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isUserDeleted( int u_ID ) {
        bool deleted = false;
        if (userExists( u_ID )) {
            try {
                user ur = getUserByID( u_ID );

                MOE_DB.users.DeleteOnSubmit( ur );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try/catch
        }//if

        return deleted;
    }//isUserdeleted



    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public user getUserByID( int id ) {
        if (userExists( id )) {
            try {
                user ur = (from u in MOE_DB.users
                           where u.user_ID == id
                           select u).Single();

                if (ur != null)
                    return ur;
                else
                    return null;
            }
            catch (Exception ex) {
                return null;
            }//try catch
        }//if
        return null;
    }//getUserbyid



    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public user getUserByEmail( string email ) {

        try {
            user ur = (from u in MOE_DB.users
                       where u.email == email
                       select u).Single();

            if (ur != null)
                return ur;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch

        return null;
    }//getUserbyid




    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public user[] getAllUsers() {

        try {
            user[] ur = (from u in MOE_DB.users
                         select u).ToArray();

            if (ur != null)
                return ur;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch
    }//getAllUsers






    /******************************************************
   *                  Lesson Plan Methods
   *
   * ****************************************************/

    bool lessonplanExist( int id ) {
        try {
            lessonPlan plan = (from p in MOE_DB.lessonPlans
                               where p.lessonPlan_ID == id
                               select p).Single();
            if (plan != null)
                return true;
            else
                return false;
        }
        catch (Exception ex) {
            return false;
            ;
        }

    }//lessonplanExist


    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public bool isLessonplanDeleted( int id ) {
        bool deleted = false;
        if (lessonplanExist( id )) {
            try {
                lessonPlan p = getLessonplanByID( id );
                MOE_DB.lessonPlans.DeleteOnSubmit( p );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//tryt catch 

        }//if

        return deleted;
    }//isLessonplanDeleted


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isLessonplanAdded( int t_ID, int s_ID, DateTime Sdate, DateTime fdate, string duration, string topic, string stopic,
                                    int formClass_ID, string preKnow, string content, string intro, string eval ) {
        try {

            lessonPlan newPlan = new lessonPlan() {
                teacher_ID = t_ID,
                subject_ID = s_ID,
                startDate = Sdate,
                endDate = fdate,
                duration = duration,
                topic = topic,
                subtopic = stopic,
                formClass_ID = formClass_ID,
                PreviousKnowledge = preKnow,
                plannedContent = content,
                introduction = intro,
                evaluations = eval
            };

            MOE_DB.lessonPlans.InsertOnSubmit( newPlan );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }//try catch
    }//addLessonplan


    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public bool isLessonplanUpdated( int lessonplan_ID, int t_ID, int s_ID, DateTime Sdate, DateTime fdate, string duration, string topic, string stopic,
                                   int formClass_ID, string preKnow, string content, string intro, string eval ) {
        try {

            lessonPlan updatedPlan = new lessonPlan() {
                teacher_ID = t_ID,
                subject_ID = s_ID,
                startDate = Sdate,
                endDate = fdate,
                duration = duration,
                topic = topic,
                subtopic = stopic,
                formClass_ID = formClass_ID,
                PreviousKnowledge = preKnow,
                plannedContent = content,
                introduction = intro,
                evaluations = eval,
                lessonPlan_ID = lessonplan_ID
            };

            lessonPlan oldPlan = (from p in MOE_DB.lessonPlans
                                  where p.lessonPlan_ID == lessonplan_ID
                                  select p).Single();

            oldPlan = updatedPlan;
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }//try catch
    }//isLessonplanUpdated




    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public lessonPlan getLessonplanByID( int id ) {
        if (lessonplanExist( id )) {
            try {
                lessonPlan plan = (from p in MOE_DB.lessonPlans
                                   where p.lessonPlan_ID == id
                                   select p).Single();
                if (plan != null)
                    return plan;
                else
                    return null;

            }
            catch (Exception ex) {
                return null;
            }//try catch
        }//if
        return null;
    }//getlessonplanByID



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public lessonPlan[] getAllLessonplans() {

        try {
            lessonPlan[] plans = (from p in MOE_DB.lessonPlans
                                  select p).ToArray();
            if (plans != null)
                return plans;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch        
    }//getAllLessonplans


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public lessonPlan[] getLessonplansByTeacher( int teacherID ) {
        try {
            lessonPlan[] plans = (from p in MOE_DB.lessonPlans
                                  where p.teacher_ID == teacherID
                                  select p).ToArray();
            if (plans != null)
                return plans;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch        
    }//getLessonplanByTeacher


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public lessonPlan[] getLessonplansBySubject( int subjectID ) {
        try {
            lessonPlan[] plans = (from p in MOE_DB.lessonPlans
                                  where p.subject_ID == subjectID
                                  select p).ToArray();
            if (plans != null)
                return plans;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch        
    }//getLessonplanBySubject


    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public lessonPlan[] getLessonplansBySubjectNTeacher( int subjectID, int teacherID ) {
        try {
            lessonPlan[] plans = (from p in MOE_DB.lessonPlans
                                  where p.subject_ID == subjectID && p.teacher_ID == teacherID
                                  select p).ToArray();
            if (plans != null)
                return plans;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch        
    }//getLessonplanBySubjectNTeacher

    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public lessonPlan[] getLessonplanByTopic( string topic ) {
        try {
            lessonPlan[] plans = (from p in MOE_DB.lessonPlans
                                  where p.topic == topic || p.subtopic == topic
                                  select p).ToArray();
            if (plans != null)
                return plans;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }//try catch        
    }


    //************************************************************************************
    //**                             General Objectives
    //************************************************************************************       


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isGeneralObjectiveAdded( string GObjective ) {
        if (!generalObjectiveExists( GObjective )) {
            try {
                generalObjective go = new generalObjective() {
                    objective = GObjective
                };

                MOE_DB.generalObjectives.InsertOnSubmit( go );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isGeneralObjectveAdded



    [WebMethod]
    public bool isLessonplanGeneralObjectiveAdded(int lessonplanID, int generalObjectiveID) {
        lessonplanGeneralobjective lpgo = new lessonplanGeneralobjective() {
            lessonPlan_ID = lessonplanID,
            generalObjective_ID = generalObjectiveID
        };

        try {
            MOE_DB.lessonplanGeneralobjectives.InsertOnSubmit( lpgo );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isLessonplanGeneralObjectiveAdded


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public generalObjective[] getAllGeneralObjectives() {
        try {
            generalObjective[] Gobjectives = (from go in MOE_DB.generalObjectives
                                              select go).ToArray();

            if (Gobjectives != null)
                return Gobjectives;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getAllGeneralObjectives


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public generalObjective getGeneralObjectiveByID( int id ) {
        try {
            generalObjective gobjective = (from go in MOE_DB.generalObjectives
                                           where go.generalObjective_ID == id
                                           select go).Single();
            if (gobjective != null)
                return gobjective;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getGeneralObjectiveByID


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool generalObjectiveExists( string objective ) {

        generalObjective go = (from gO in MOE_DB.generalObjectives
                               where gO.objective == objective
                               select gO).Single();
        if (go != null)
            return true;
        else
            return false;
    }//generalObjectiveExists

    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isGeneralObjectiveUpdated( int id, string objective ) {
        if (!generalObjectiveExists( objective )) {

            generalObjective GO = (from gO in MOE_DB.generalObjectives
                                   where gO.generalObjective_ID == id
                                   select gO).Single();
            GO.objective = objective;
            MOE_DB.SubmitChanges();
            return true;
        }//if
        else
            return false;

    }//isGeneralObjectiveUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isGeneralObjectiveDeleted( int id, string objective ) {
        if (generalObjectiveExists( objective )) {
            generalObjective GO = getGeneralObjectiveByID( id );

            MOE_DB.generalObjectives.DeleteOnSubmit( GO );
            try {
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        else
            return false;
    }//isGeneralObjectiveDeleted




    [WebMethod( Description = "Description: "
                                 + "Arguments:  "
                                 + "precondition: "
                                 + "postcondition: "
                                 + "Example: " )]
    public bool isLessonPlanGeneralObjectiveAdded( int lessonplanID, int generalObjectiveID ) {

        lessonplanGeneralobjective lpgo = new lessonplanGeneralobjective() {
            lessonPlan_ID = lessonplanID,
            generalObjective_ID = generalObjectiveID
        };

        MOE_DB.lessonplanGeneralobjectives.InsertOnSubmit( lpgo );
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isLessonplanGeneralObjectiveAdded


    [WebMethod( Description = "Description: "
                                 + "Arguments:  "
                                 + "precondition: "
                                 + "postcondition: "
                                 + "Example: " )]
    public generalObjective[] getGeneralObjectiveBylessonplanID( int planID ) {
        generalObjective[] go = (from GenOb in MOE_DB.generalObjectives
                                 join planObjective in MOE_DB.lessonplanGeneralobjectives on GenOb.generalObjective_ID equals planObjective.generalObjective_ID
                                 where planObjective.lessonPlan_ID == planID
                                 select GenOb).ToArray();

        if (go != null)
            return go;
        else
            return null;

    }//getGeneralObjectiveBylessonplanID




    //*************************************************************************************************
    //**                      Specific Objectives


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool isSpecificObjectiveAdded( string SObjective ) {
        if (!specificObjectiveExists( SObjective )) {

            try {
                specificObjective so = new specificObjective() {
                    objective = SObjective
                };

                MOE_DB.specificObjectives.InsertOnSubmit( so );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isSpecificObjectveAdded


    [WebMethod]
    public bool isLessonplanSpecificobjectiveAdded( int lessonplanID, int specificobjectiveID ) {
        lessonplanSpecificobjective lpso = new lessonplanSpecificobjective() {
            lessonPlan_ID = lessonplanID,
            specificObjective_ID = specificobjectiveID
        };
        try {
            MOE_DB.lessonplanSpecificobjectives.InsertOnSubmit( lpso );
                MOE_DB.SubmitChanges();
            return true;
        }catch(Exception ex){
            return false;
        }

    }//isLessonplanSpecificAdded


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public specificObjective[] getAllSpecificObjectives() {
        try {
            specificObjective[] Sobjectives = (from so in MOE_DB.specificObjectives
                                               select so).ToArray();

            if (Sobjectives != null)
                return Sobjectives;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getAllSpecificObjectives


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public specificObjective getSpecificObjectiveByID( int id ) {
        try {
            specificObjective sobjective = (from so in MOE_DB.specificObjectives
                                            where so.specificObjective_ID == id
                                            select so).Single();
            if (sobjective != null)
                return sobjective;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getSpecificObjectiveByID

    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool specificObjectiveExists( string objective ) {

        specificObjective so = (from sO in MOE_DB.specificObjectives
                                where sO.objective == objective
                                select sO).Single();
        if (so != null)
            return true;
        else
            return false;
    }//specificObjectiveExists


    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isSpecificObjectiveUpdated( int id, string objective ) {
        if (!specificObjectiveExists( objective )) {

            specificObjective SO = (from sO in MOE_DB.specificObjectives
                                    where sO.specificObjective_ID == id
                                    select sO).Single();
            SO.objective = objective;
            MOE_DB.SubmitChanges();
            return true;
        }//if
        else
            return false;

    }//isSpecificObjectiveUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isSpecificObjectiveDeleted( int id, string objective ) {
        if (specificObjectiveExists( objective )) {
            specificObjective SO = getSpecificObjectiveByID( id );

            MOE_DB.specificObjectives.DeleteOnSubmit( SO );
            try {
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        else
            return false;
    }//isSpecificObjectiveDeleted


    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public specificObjective[] getSpecificObjectiveBylessonplanID( int planID ) {
        specificObjective[] so = (from SpecObjec in MOE_DB.specificObjectives
                                  join planObjective in MOE_DB.lessonplanSpecificobjectives
                                  on SpecObjec.specificObjective_ID equals planObjective.specificObjective_ID
                                  where planObjective.lessonPlan_ID == planID
                                  select SpecObjec).ToArray();


        if (so != null)
            return so;
        else
            return null;

    }//getSpecifiicObjectiveBylessonplanID






    //*********************************************************************************************
    //**                            Methods

    [WebMethod( Description = "Description: "
                            + "Arguments:  "
                            + "precondition: "
                            + "postcondition: "
                            + "Example: " )]
    public bool methodExists( string method ) {
        method meth = (from m in MOE_DB.methods
                       where m.name == method
                       select m).Single();

        if (meth != null)
            return true;
        else
            return false;
    }//methodExixts



    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isMethodAdded( string method ) {
        if (!methodExists( method )) {
            try {
                method m = new method() {
                    name = method
                };

                MOE_DB.methods.InsertOnSubmit( m );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isMethodAdded



    [WebMethod]
    public bool isLessonplanMethodAdded( int lessonplanID, int methodID ) {
        lessonPlanMethod lpm = new lessonPlanMethod() {
            lessonPlan_ID = lessonplanID,
            method_ID = methodID
        };
        try {
            MOE_DB.lessonPlanMethods.InsertOnSubmit( lpm );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isLessoplanMethodAdded


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public method[] getAllMethods() {
        try {
            method[] m = (from so in MOE_DB.methods
                          select so).ToArray();

            if (m != null)
                return m;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getAllMethods


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public method getMethodByID( int id ) {
        try {
            method m = (from so in MOE_DB.methods
                        where so.method_ID == id
                        select so).Single();
            if (m != null)
                return m;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getMethodByID




    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isMethodUpdated( int id, string method ) {
        if (methodExists( method )) {

            method m = (from sO in MOE_DB.methods
                        where sO.method_ID == id
                        select sO).Single();
            m.name = method;
            MOE_DB.SubmitChanges();
            return true;
        }//if
        else
            return false;

    }//isMethodUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isMethodeDeleted( int id ) {

        method m = getMethodByID( id );

        MOE_DB.methods.DeleteOnSubmit( m );
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }

    }//isMethodDeleted







    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public method[] getMethodsByLessonplanID( int planID ) {

        method[] meths = (from meth in MOE_DB.methods
                          join planMeth in MOE_DB.lessonPlanMethods
                          on meth.method_ID equals planMeth.method_ID
                          where planMeth.lessonPlan_ID == planID
                          select meth).ToArray();

        if (meths != null)
            return meths;
        else
            return null;

    }//getMethodsByLessonplanID






    //*************************************************************************************************
    //**                                  Instructional matrials


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool instructionalMaterialExists( string material ) {
        instructionalMaterial im = (from m in MOE_DB.instructionalMaterials
                                    where m.material == material
                                    select m).Single();
        if (im != null)
            return true;
        else
            return false;
    }//instructionalMaterialExixts


    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isInstructionalMaterialAdded( string material ) {
        if (!instructionalMaterialExists( material )) {

            try {
                instructionalMaterial im = new instructionalMaterial() {
                    material = material
                };

                MOE_DB.instructionalMaterials.InsertOnSubmit( im );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isInstructionalMaterialAdded


    [WebMethod]
    public bool isLessonplanMaterialAdded( int lessonplanID, int materialID ) {
        lessonplanInstructionalMaterial lpim = new lessonplanInstructionalMaterial() {
            lessonPlan_ID = lessonplanID,
            instructionalMaterials_ID = materialID
        };
        try {
            MOE_DB.lessonplanInstructionalMaterials.InsertOnSubmit( lpim );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }

    }//isLessonplanMaterialAdded



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public instructionalMaterial[] getAllInstructionalMaterials() {
        try {
            instructionalMaterial[] im = (from so in MOE_DB.instructionalMaterials
                                          select so).ToArray();

            if (im != null)
                return im;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getAllinstructionalMaterials


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public instructionalMaterial getInstructionalMaterialByID( int id ) {
        try {
            instructionalMaterial m = (from so in MOE_DB.instructionalMaterials
                                       where so.instructionalMaterials_ID == id
                                       select so).Single();
            if (m != null)
                return m;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getInstructionalMaterialByID


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isInstructionalMaterialUpdated( int id, string material ) {
        if (!instructionalMaterialExists( material )) {

            instructionalMaterial m = (from sO in MOE_DB.instructionalMaterials
                                       where sO.instructionalMaterials_ID == id
                                       select sO).Single();
            m.material = material;
            MOE_DB.SubmitChanges();
            return true;
        }//if
        else
            return false;

    }//isinstructionalMaterialUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isInstructionalMaterialDeleted( int id ) {

        instructionalMaterial m = getInstructionalMaterialByID( id );

        MOE_DB.instructionalMaterials.DeleteOnSubmit( m );
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isInstructionalMaterialDeleted




    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public instructionalMaterial[] getInstructionalMaterialsByLessonplanID( int planID ) {

        instructionalMaterial[] materi = (from mat in MOE_DB.instructionalMaterials
                                          join planMat in MOE_DB.lessonplanInstructionalMaterials
                                          on mat.instructionalMaterials_ID equals planMat.instructionalMaterials_ID
                                          where planMat.lessonPlan_ID == planID
                                          select mat).ToArray();

        if (materi != null)
            return materi;
        else
            return null;
    }//getInstructionalMaterialsByLessonplanID







    //*************************************************************************************************
    //**                                plan Activities



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public bool planactivityExists( string activity ) {
        planactivity PA = (from pact in MOE_DB.planactivities
                           where pact.activity == activity
                           select pact).Single();
        if (PA != null)
            return true;
        else
            return false;
    }//planactivityExixts


    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isPlanactivityAdded( string activity ) {
        if (!planactivityExists( activity )) {

            try {
                planactivity pa = new planactivity() {
                    activity = activity
                };

                MOE_DB.planactivities.InsertOnSubmit( pa );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isplanactivityAdded


    [WebMethod]
    public bool isLessonplanActivityAdded( int lessonplanID, int activityID ) {
        lessonplanPlanactivite lpa = new lessonplanPlanactivite() {
            lessonPlan_ID = lessonplanID,
            planActivity_ID = activityID
        };

        try {
            MOE_DB.lessonplanPlanactivites.InsertOnSubmit( lpa );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch(Exception ex) {
            return false;
        }
    }//isLessonplanActivityAdded



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public planactivity[] getAllPlanactivities() {
        try {
            planactivity[] PA = (from pas in MOE_DB.planactivities
                                 select pas).ToArray();

            if (PA != null)
                return PA;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getPlanactivities


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public planactivity getPlanactivityByID( int id ) {
        try {
            planactivity PA = (from pa in MOE_DB.planactivities
                               where pa.planActivity_ID == id
                               select pa).Single();
            if (PA != null)
                return PA;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getplanactivityByID


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isPlanactivityUpdated( int id, string activity ) {
        if (!planactivityExists( activity )) {

            planactivity PA = (from pa in MOE_DB.planactivities
                               where pa.planActivity_ID == id
                               select pa).Single();
            PA.activity = activity;
            try {
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }

        }//if
        else
            return false;

    }//isPlanactivityUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isPlanactivityDeleted( int id ) {

        planactivity PA = getPlanactivityByID( id );

        MOE_DB.planactivities.DeleteOnSubmit( PA );
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isPlanactivityDeleted




    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public planactivity[] getLessonplanActiviesByLessonplanID( int planID ) {

        planactivity[] activities = (from acti in MOE_DB.planactivities
                                     join planActies in MOE_DB.lessonplanPlanactivites
                                     on acti.planActivity_ID equals planActies.planActivity_ID
                                     where planActies.lessonPlan_ID == planID
                                     select acti).ToArray();

        if (activities != null)
            return activities;
        else
            return null;
    }//getlessonplanActivitesByLessonplanID




    //*************************************************************************************************
    //**                      Plan   Steps

    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public bool stepExists( string pstep ) {
        step st = (from s in MOE_DB.steps
                   where s.lessonStep == pstep
                   select s).Single();
        if (st != null)
            return true;
        else
            return false;
    }//stepExixts


    [WebMethod( Description = "Description: "
                               + "Arguments:  "
                               + "precondition: "
                               + "postcondition: "
                               + "Example: " )]
    public bool isStepAdded( string LessonStep, int stepNumb ) {
        if (!stepExists( LessonStep )) {

            try {
                step st = new step() {
                    lessonStep = LessonStep,
                    stepNumber = stepNumb
                };

                MOE_DB.steps.InsertOnSubmit( st );
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }//try catch
        }
        else
            return false;
    }// isStepAdded


    [WebMethod]
    public bool isLessonStepAdded(int stepID, int LessonplanID) {

        lesssonPlanStep lps = new lesssonPlanStep() {
            lessonPlan_ID = LessonplanID,
            step_ID = stepID
        };
        try {
            MOE_DB.lesssonPlanSteps.InsertOnSubmit( lps );
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isLessonStepAdded



    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public step[] getAllSteps() {
        try {
            step[] Steps = (from s in MOE_DB.steps
                            select s).ToArray();

            if (Steps != null)
                return Steps;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getAllSteps


    [WebMethod( Description = "Description: "
                              + "Arguments:  "
                              + "precondition: "
                              + "postcondition: "
                              + "Example: " )]
    public step getStepByID( int id ) {
        try {
            step st = (from s in MOE_DB.steps
                       where s.step_ID == id
                       select s).Single();
            if (st != null)
                return st;
            else
                return null;
        }
        catch (Exception ex) {
            return null;
        }
    }//getStepByID


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isStepUpdated( int id, string LessonStep, int stepNumb ) {
        if (!stepExists( LessonStep )) {

            step st = (from s in MOE_DB.steps
                       where s.step_ID == id
                       select s).Single();
            st.lessonStep = LessonStep;
            st.stepNumber = stepNumb;
            try {
                MOE_DB.SubmitChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }

        }//if
        else
            return false;

    }//isStepUpdated


    [WebMethod( Description = "Description: "
                                + "Arguments:  "
                                + "precondition: "
                                + "postcondition: "
                                + "Example: " )]
    public bool isStepDeleted( int id ) {

        step st = getStepByID( id );

        MOE_DB.steps.DeleteOnSubmit( st );
        try {
            MOE_DB.SubmitChanges();
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }//isStepDeleted




    [WebMethod( Description = "Description: "
                             + "Arguments:  "
                             + "precondition: "
                             + "postcondition: "
                             + "Example: " )]
    public step[] getStepsByLessonplanID( int planID ) {

        step[] steps = (from st in MOE_DB.steps
                        join planSt in MOE_DB.lesssonPlanSteps
                        on st.step_ID equals planSt.step_ID
                        where planSt.lessonPlan_ID == planID
                        select st).ToArray();

        if (steps != null)
            return steps;
        else
            return null;
    }//getStepsByLessonplanID


    //******************************************************
    //


















































































    /******************************************************
   *                  
   *
   * ****************************************************/














}//class

