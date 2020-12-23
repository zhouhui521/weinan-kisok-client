using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.patient
{
    public class PatientService
    {
        
        public string getHosPatient(string hosId)
        {
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(hosId, url + "api/patient/hosInfo");
            return result;
        }
    }
}
