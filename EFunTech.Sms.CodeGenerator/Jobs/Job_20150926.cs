﻿using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class Job_20150926 : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        public void Execute()
        {
            List<IJob> jobs = new List<IJob>();


            //jobs.Add(new SingleGridPageGenerator
            //{
            //    SchemaType = typeof(SendMessageStatistic),
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //    JsControllerName = "SectorSendMessageStatistic",
            //    ColumnWidth = 4,
            //});

            //jobs.Add(new SingleGridPageGenerator
            //{
            //    SchemaType = typeof(SendMessageHistory),
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //    JsControllerName = "SectorSendMessageHistory",
            //    ColumnWidth = 4,
            //});

            jobs.Add(new SingleGridPageGenerator
            {
                SchemaType = typeof(SendMessageStatistic),
                PortalDir = PortalDir,
                Overwrite = Overwrite,
                JsControllerName = "MemberSendMessageStatistic",
                ColumnWidth = 4,
            });

            //jobs.Add(new SingleGridPageGenerator
            //{
            //    SchemaType = typeof(SendMessageHistory),
            //    PortalDir = PortalDir,
            //    Overwrite = Overwrite,
            //    JsControllerName = "MemberSendMessageHistory",
            //    ColumnWidth = 4,
            //});

            
            jobs.ForEach(job => job.Execute());
        }
    }
}
