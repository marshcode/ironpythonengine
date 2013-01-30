﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ironpythonengine.test
{
    using NUnit.Framework;
    using ironpythonengine;
    using System.IO;

    
    class EngineTestFramework
    {

        protected IronPythonEngine ipe;


        [Test]
        public void test_constructor()
        {
            Assert.IsNotNull(ipe.get_engine());
            Assert.IsNotNull(ipe.get_scope());
        }

        [Test]
        public void test_run_expression()
        {
            int x = ipe.get_engine().Execute<int>("3+4");
            Assert.AreEqual(x, 7);
        }

        [Test]
        public void test_multi_session_expression()
        {

            
            ipe.get_engine().Execute("x=4", ipe.get_scope());
            Console.Write( ipe.get_scope().GetVariable("x") );

            int x = ipe.get_engine().Execute<int>("x-1", ipe.get_scope());
            Assert.AreEqual(x, 3);
        }


        [Test]
        public void test_simiple_variable_injection()
        {
            ipe.get_scope().SetVariable("name", "Dave");
            string str = ipe.get_engine().Execute<string>(@" 
def blah():
    return 'Hello, {}!'.format(name);
blah()
", ipe.get_scope());
            Assert.AreEqual(str, "Hello, Dave!");
        
        }

        public void perform_socket_access(string ip, string port) {
           
        }

        public string perform_dns_service() {
            return ipe.get_engine().Execute<string>("socket.gethostbyname('localhost')");
        }

        public void perform_read_file_access(string path) {
            string the_path = System.IO.Path.GetTempFileName();
            string content = "whatever whatever whatever";
            
            System.IO.File.WriteAllText(the_path, content);

            ipe.get_scope().SetVariable("the_path", the_path);
            string file_content = ipe.get_engine().Execute(@"
with open(the_path) as f:
    file_content = f.read()
");

            Assert.AreEqual(ipe.get_scope().GetVariable("file_content"),
                            content);


        }

        public void perform_write_file_access(string path, string content) {
            ipe.get_scope().SetVariable("the_path", path);
            ipe.get_scope().SetVariable("content",  content);

            ipe.get_engine().Execute(@"
with open(the_path) as f:
    f.write( content )
");
        }

    }

    [TestFixture]
    class UnsecuredEngineTest : EngineTestFramework {

        [TestFixtureSetUp]
        public void setup() {

            this.ipe = new ironpythonengine.UnrestrictedIronPythonEngine();
        }

    }



}
