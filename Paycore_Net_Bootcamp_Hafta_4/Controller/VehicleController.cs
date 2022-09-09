using  Paycore_Net_Bootcamp_Hafta_4.Context;
using  Paycore_Net_Bootcamp_Hafta_4.Models;

using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Paycore_Net_Bootcamp_Hafta_4.KMeanAlgorithm;

namespace Paycore_Net_Bootcamp_Hafta_4.Controller
{
    [Route("api/[controller]")]
        [ApiController]
    public class VehicleController  : ControllerBase
    {


        private readonly IMapperSession<Vehicle> sessionVehicle;
        private readonly IMapperSession<Container> sessionContainer;


        public VehicleController(IMapperSession<Vehicle> sessionVehicle, IMapperSession<Container> sessionContainer)
        {

            this.sessionVehicle = sessionVehicle;
            this.sessionContainer = sessionContainer;
        }
        ////get containers that own vehicle
        //[HttpGet("GetContainersByVehicleId")]
        //public List<Container> GetContainersByVehicleId(int id)
        //{
        //    List<Container> result = sessionContainer.Entities.Where(x => x.VehicleId == id).ToList();

        //    return result;
        //}
        //get cluster of container by given  vehicle id and cluster number 
        [HttpGet("GetClustersByVehicleId")]
        public ActionResult<List<List<Container>>> GetClustersByVehicleId(int id,int n)
        {
            List<Container> containers = sessionContainer.Entities.Where(x => x.VehicleId == id).ToList();
            List<List<Container>> result = new List<List<Container>>();
            if (containers.Count < n)
            {
                return BadRequest("N must be smaller than containers.");
            }

            var diziFormu = containers.Select(n => new double[] { n.Latitude, n.Longitude }) // her bir satır için double dizi dön
                .ToArray(); // double dizilerinden bir dizi oluştur

            var sonuc = KMeanAlgorithmClass.KMeans(diziFormu, n);

            for (int i = 0; i < n; i++)
            {
                result.Add(new List<Container>());
            }
            int index = 0;



            foreach (var item in sonuc)
            {

                result[item].Add(containers[index++]);
                Console.Write(item + ",");
            }

           
            return result;
        }
        ////get all vehicles
        //// GET: api/<VehicleController>
        //[HttpGet]
        //public IEnumerable<Vehicle> Get()
        //{
        //    return sessionVehicle.Entities;
        //}
        ////get vehicle by given id
        //// GET api/<VehicleController>/5
        //[HttpGet("{id}")]
        //public Vehicle Get(int id)
        //{
        //    return sessionVehicle.Entities.Where(x => x.Id == id).FirstOrDefault();
        //}
        ////create new vehicle
        //// POST api/<VehicleController>
        //[HttpPost]
        //public void Post([FromBody] Vehicle vehicle)
        //{
        //    try
        //    {
        //        sessionVehicle.BeginTransaction();
        //        sessionVehicle.Save(vehicle);
        //        sessionVehicle.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        sessionVehicle.Rollback();
        //        Log.Error(ex, "vehicles Insert Error");
        //    }
        //    finally
        //    {
        //        sessionVehicle.CloseTransaction();
        //    }
        //}
        ////update existing vehicle
        //// PUT api/<VehicleController>/5
        //[HttpPut]
        //public ActionResult<Vehicle> Put([FromBody] Vehicle request)
        //{
        //    Vehicle vehicle = sessionVehicle.Entities.Where(x => x.Id == request.Id).FirstOrDefault();
        //    if (vehicle == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        sessionVehicle.BeginTransaction();

        //        vehicle.VehicleName = request.VehicleName;
        //        vehicle.VehiclePlate = request.VehiclePlate;


        //        sessionVehicle.Update(vehicle);

        //        sessionVehicle.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        sessionVehicle.Rollback();
        //        Log.Error(ex, "vehicle Insert Error");
        //    }
        //    finally
        //    {
        //        sessionVehicle.CloseTransaction();
        //    }


        //    return Ok();
        //}
        ////delete existing  vehicle
        //[HttpDelete("{id}")]
        //public ActionResult<Vehicle> Delete(int id)
        //{
        //    Vehicle vehicle = sessionVehicle.Entities.Where(x => x.Id == id).FirstOrDefault();
        //    if (vehicle == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        sessionVehicle.BeginTransaction();
        //        sessionVehicle.Delete(vehicle);
        //        sessionVehicle.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        sessionVehicle.Rollback();
        //        Log.Error(ex, "vehicle Insert Error");
        //    }
        //    finally
        //    {
        //        sessionVehicle.CloseTransaction();
        //    }

        //    return Ok();
        //}
    }
}

