using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Browser.SignaturePrinters;

namespace Core.Browser
{
    public class AssemblyBrowser
    {
        private readonly Assembly _mainAssembly;
        private readonly List<ISignaturePrinter> _printers;
        
        public AssemblyBrowser(string fileName)
        {
            _mainAssembly = Assembly.LoadFrom(fileName);
            _printers = new List<ISignaturePrinter>
            {
                new MethodSignaturePrinter(),
                new FieldSignaturePrinter(),
                new PropertySignaturePrinter()
            };
        }

        public List<string> GetNamespaces()
        {
            return _mainAssembly.GetTypes()
                .Select(t => t.Namespace)
                .Where(n => n != null)
                .Distinct()
                .Where(n => !n.StartsWith("System"))
                .Where(n => !n.StartsWith("Microsoft"))
                .ToList();
        }

        public List<string> GetTypes(string ns)
        {
            return _mainAssembly.GetTypes()
                .Where(t => t.Namespace != null)
                .Where(t => t.Namespace == ns)
                .Select(t => t.Name)
                .Where(n => !n.Contains('<'))
                .ToList();
        }

        public List<string> GetMethods(string ns, string tp)
        {
            return _mainAssembly.GetTypes()
                .Where(t => t.Namespace != null)
                .Where(t => t.Namespace == ns)
                .First(t => t.Name == tp)
                .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(m => !m.Name.Contains('<'))
                .Select(GetSignature)
                .ToList();
        }

        private string GetSignature(MemberInfo info)
        {
            foreach (var printer in _printers)
            {
                if (printer.CanPrint(info))
                {
                    return printer.Print(info);
                }
            }

            return "<some member>";
        }
    }
}