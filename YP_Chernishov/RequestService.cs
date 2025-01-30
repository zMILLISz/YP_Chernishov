using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YP_Chernishov
{
    public class RequestService
    {
        private Request _currentRequests;

        public RequestService(Request request)
        {
            _currentRequests = request ?? new Request();
        }

        public void SetPatient(int? patientId)
        {
            if (patientId != null)
                _currentRequests.RequestPatient = patientId.Value;
        }

        public void SetSpecialist(int? specialistId)
        {
            if (specialistId != null)
                _currentRequests.RequestSpecialist = specialistId.Value;
        }

        public void SetRequestData(DateTime date, TimeSpan time)
        {
            _currentRequests.RequestData = date + time;
        }

        public Request CurrentRequest => _currentRequests;

        public bool IsDoctorAvailable(int specialistId, DateTime date)
        {
            var existingRequest = YP_ChernishovEntities.GetContext().Request
                .Any(r => r.RequestPatient == _currentRequests.RequestPatient &&
                r.RequestSpecialist == specialistId &&
                DbFunctions.TruncateTime(r.RequestData) == date.Date);

            return !existingRequest;
        }

        public void SaveRequest()
        {
            if (_currentRequests.RequestId == 0)
                YP_ChernishovEntities.GetContext().Request.Add(_currentRequests);

            YP_ChernishovEntities.GetContext().SaveChanges();
        }
    }
}
