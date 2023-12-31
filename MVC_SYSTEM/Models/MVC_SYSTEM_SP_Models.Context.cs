﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MVC_SYSTEM_SP_Models : DbContext
    {
        public MVC_SYSTEM_SP_Models()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual int sp_AddTotblPkjmastApp(Nullable<long> fldFileID)
        {
            var fldFileIDParameter = fldFileID.HasValue ?
                new ObjectParameter("fldFileID", fldFileID) :
                new ObjectParameter("fldFileID", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AddTotblPkjmastApp", fldFileIDParameter);
        }
    
        public virtual int sp_AddTotblUserIDApp(Nullable<long> fldFileID)
        {
            var fldFileIDParameter = fldFileID.HasValue ?
                new ObjectParameter("fldFileID", fldFileID) :
                new ObjectParameter("fldFileID", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AddTotblUserIDApp", fldFileIDParameter);
        }
    
        public virtual ObjectResult<sp_RptBulPenPekLad_Result> sp_RptBulPenPekLad(Nullable<int> selection, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string kdrytan, Nullable<int> month, Nullable<int> year)
        {
            var selectionParameter = selection.HasValue ?
                new ObjectParameter("Selection", selection) :
                new ObjectParameter("Selection", typeof(int));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var kdrytanParameter = kdrytan != null ?
                new ObjectParameter("Kdrytan", kdrytan) :
                new ObjectParameter("Kdrytan", typeof(string));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptBulPenPekLad_Result>("sp_RptBulPenPekLad", selectionParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, kdrytanParameter, monthParameter, yearParameter);
        }
    
        public virtual ObjectResult<sp_RptBulPenPekLadSum_Result> sp_RptBulPenPekLadSum(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string kdrytan, Nullable<int> month, Nullable<int> year)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var kdrytanParameter = kdrytan != null ?
                new ObjectParameter("Kdrytan", kdrytan) :
                new ObjectParameter("Kdrytan", typeof(string));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptBulPenPekLadSum_Result>("sp_RptBulPenPekLadSum", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, kdrytanParameter, monthParameter, yearParameter);
        }
    
        public virtual ObjectResult<sp_RptLapMakKer_Result> sp_RptLapMakKer(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> month, Nullable<int> year, string noPkj)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var noPkjParameter = noPkj != null ?
                new ObjectParameter("NoPkj", noPkj) :
                new ObjectParameter("NoPkj", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptLapMakKer_Result>("sp_RptLapMakKer", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter, noPkjParameter);
        }
    
        public virtual ObjectResult<sp_RptTransPek_Result> sp_RptTransPek(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string month, Nullable<int> year)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month != null ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(string));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptTransPek_Result>("sp_RptTransPek", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter);
        }
    }
}
