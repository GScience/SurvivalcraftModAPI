using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;

namespace SurvivalcraftModAPIInstaller
{
    class Program
    {
        private static string scVersion = "2.0.30.0";
        private static string modAPIVersion = "1.0.0.0";

        static void Main(string[] args)
        {
            string path;
            string modAPIPath;

            //加载Survivalcraft.exe
            if (args.Length == 0)
            {
                System.Console.Out.Write("请输入Survivalcraft.exe的目录\n");
                path = System.Console.In.ReadLine();

                System.Console.Out.Write("请输入ModAPI.dll的目录\n");
                modAPIPath = System.Console.In.ReadLine();
            }
            else
            {
                path = args[0];
                modAPIPath = args[1];
            }

            AssemblyDefinition scAssembiy = AssemblyDefinition.ReadAssembly(path);
            AssemblyDefinition modAPIAssembiy = AssemblyDefinition.ReadAssembly(modAPIPath);

            //判断版本
            if (scAssembiy.Name.Version != new Version(scVersion))
            {
                System.Console.Out.Write("不支持的版本！仅支持" + scVersion + "\n");

                return;
            }
            //开始注入类（把自己的ModAPI类注入到SC里）

            //开始修改代码
            foreach (Mono.Cecil.TypeDefinition scClass in scAssembiy.MainModule.Types)
            {
                if (scClass.Namespace != "Game")
                    continue;

                //破解游戏
                if (scClass.Name == "MarketplaceManager")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "UpdateLicence")
                        {
                            //替换方法
                            MethodBody breakBody = new MethodBody(scMethod);
                            breakBody.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));

                            //寻找变量
                            foreach (FieldDefinition field in scClass.Fields)
                            {
                                if (field.Name == "m_isTrialMode")
                                {
                                    breakBody.Instructions.Add(Instruction.Create(OpCodes.Stsfld, field));
                                    break;
                                }
                            }
                            breakBody.Instructions.Add(Instruction.Create(OpCodes.Ret));
                            scMethod.Body = breakBody;
                        }
                    }
                }

                //游戏初始化注入
                if (scClass.Name == "FrontendManager")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "Initialize")
                        {
                            Mono.Collections.Generic.Collection<Instruction> code = scMethod.Body.Instructions;

                            //找loading screen
                            foreach (TypeDefinition loadingScreenClass in scAssembiy.MainModule.Types)
                            {
                                if (loadingScreenClass.Name == "LoadingScreen")
                                {
                                    //找AddLoadAction
                                    foreach (MethodDefinition addLoadActionMethod in loadingScreenClass.Methods)
                                    {
                                        if (addLoadActionMethod.Name == "AddLoadAction")
                                        {
                                            code.Insert(6, Instruction.Create(OpCodes.Ldloc_0));
                                            code.Insert(6, Instruction.Create(OpCodes.Callvirt, addLoadActionMethod));
                                            //寻找ModAPI中的变量
                                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                                            {
                                                if (modType.Name == "ModAction")
                                                {
                                                    foreach (FieldDefinition field in modType.Fields)
                                                        if (field.Name == "GameInitAction")
                                                        {
                                                            FieldReference gameInitActionField = scAssembiy.MainModule.ImportReference(field.Resolve());
                                                            code.Insert(6, Instruction.Create(OpCodes.Ldsfld, gameInitActionField));
                                                            break;
                                                        }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                //BlockManager的注入
                if (scClass.Name == "BlocksManager")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "Initialize")
                        {
                            //修改循环范围
                            //移除代码
                            scMethod.Body.GetILProcessor().Remove(scMethod.Body.Instructions[5]);
                            scMethod.Body.GetILProcessor().Remove(scMethod.Body.Instructions[5]);
                            scMethod.Body.GetILProcessor().Remove(scMethod.Body.Instructions[5]);
                            scMethod.Body.GetILProcessor().Remove(scMethod.Body.Instructions[5]);

                            //寻找ModAPI中的变量
                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "BlocksManager")
                                {
                                    foreach (FieldDefinition field in modType.Fields)
                                        if (field.Name == "modBlocks")
                                        {
                                            FieldReference modBlockField = scAssembiy.MainModule.ImportReference(field.Resolve());
                                            scMethod.Body.GetILProcessor().Replace(scMethod.Body.Instructions[5], Instruction.Create(OpCodes.Ldsfld, modBlockField));
                                            break;
                                        }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            scAssembiy.Write(path.Substring(0, path.Length - 4) + "-ModAPI-" + modAPIVersion + ".exe");
        }
    }
}
