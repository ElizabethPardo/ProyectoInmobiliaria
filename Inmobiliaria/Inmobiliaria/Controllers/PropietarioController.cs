﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        

        private readonly RepositorioPropietario repositorio;
        private readonly IConfiguration config;

        public PropietarioController(IConfiguration config)
        {
            repositorio = new RepositorioPropietario(config);
            this.config = config;
        }

        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: PropietarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PropietarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorio.Alta(propietario);
                    TempData["Id"] = propietario.IdPropietario;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(propietario);
            }
        }

        // GET: PropietarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Propietario p = null;
            try
            {
                p = repositorio.ObtenerPorId(id);
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                p.Telefono = collection["Telefono"];
                repositorio.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: PropietarioController/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPass(int id, CambioClave cambio)
        {
            Propietario propietario = null;
            try
            {
                propietario = repositorio.ObtenerPorId(id);
                // verificar clave antigüa
                var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveVieja ?? "",
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                if (propietario.Clave != pass)
                {
                    TempData["Error"] = "Clave incorrecta";
                    //se rederige porque no hay vista de cambio de pass, está compartida con Edit
                    return RedirectToAction("Edit", new { id = id });
                }
                if (ModelState.IsValid)
                {
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorio.Modificacion(propietario);
                    TempData["Mensaje"] = "Contraseña actualizada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Error"] += error.ErrorMessage + "\n";
                        }
                    }
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction("Edit", new { id = id });
            }
        }

    }
}