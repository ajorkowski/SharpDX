// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Mono.Cecil;
using SharpCore.Logging;
using SharpDoc.Model;

namespace SharpDoc
{
    public class AssemblyManager : BaseAssemblyResolver 
    {
        public AssemblyManager()
        {
            Sources = new List<string>();
            References = new List<string>();
            AssemblySources = new List<NAssemblySource>();
            AssemblyReferences = new List<AssemblyDefinition>();
            AssemblyDocSources = new List<XmlDocument>();
        }

        public List<string> Sources { get; private set; }

        public List<string> References { get; private set; }

        private List<AssemblyDefinition> AssemblyReferences { get; set; }

        private List<NAssemblySource> AssemblySources { get; set; }

        private List<XmlDocument> AssemblyDocSources { get; set; }

        /// <summary>
        /// Loads all <see cref="Sources"/> and <see cref="References"/>.
        /// </summary>
        public List<NAssemblySource> Load()
        {
            // Preload references
            foreach(var assemblyRef in References)
                AssemblyReferences.Add(AssemblyDefinition.ReadAssembly(assemblyRef, new ReaderParameters(ReadingMode.Deferred)));

            // Load all sources
            foreach (var source in Sources)
                Load(source);

            // Check that all source assemblies have valid Xml associated with it
            foreach (var assemblySource in AssemblySources)
            {
                int countXmlDocFound = 0;
                XmlDocument docFound = null;
                string assemblyName = ((AssemblyDefinition) assemblySource.Assembly).Name.Name;
                foreach (var doc in AssemblyDocSources)
                {

                    var node = doc.SelectSingleNode("/doc/assembly/name");
                    if (assemblyName == node.InnerText.Trim())
                    {
                        docFound = doc;
                        countXmlDocFound++;
                    }
                }
                if (countXmlDocFound == 0)
                    Logger.Fatal("Unable to find documentation for assembly [{0}]", assemblyName);
                else if (countXmlDocFound > 1)
                    Logger.Fatal("Cannot load from multiple ([{0}]) documentation sources for assembly [{1}]", countXmlDocFound, assemblyName);

                assemblySource.Document = docFound;
            }
            return AssemblySources;
        }

        private void Load(string source)
        {
            var extension = Path.GetExtension(source);

            if (extension != null && (extension.ToLower() == ".dll" || extension.ToLower() == ".exe"))
            {
                LoadAssembly(source);
            }
            else if (extension != null && extension.ToLower() == ".xml")
            {
                LoadAssemblyDocumentation(source);
            } else
            {
                Logger.Fatal("Invalid Assembly source [{0]}]. Must be either an Assembly or a Xml comment file", source);
            }
        }

        private void LoadAssembly(string source)
        {
            var parameters = new ReaderParameters(ReadingMode.Immediate);
            parameters.AssemblyResolver = this;
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(source, parameters);
            var assemblySource = new NAssemblySource(assemblyDefinition) {Filename = source};
            AssemblySources.Add(assemblySource);
        }

        private void LoadAssemblyDocumentation(string source)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(source);

            var node = xmlDoc.SelectSingleNode("/doc/assembly/name");
            if (node == null)
                Logger.Fatal("Not valid xml documentation for source [{0}]", source);

            AssemblyDocSources.Add(xmlDoc);
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            foreach (var assemblyRef in AssemblyReferences)
            {
                if (assemblyRef.FullName == name.Name)
                    return assemblyRef;
            }
            
            return base.Resolve(name, parameters);
        }
    }
}