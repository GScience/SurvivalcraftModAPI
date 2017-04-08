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

        static int Main(string[] args)
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
            AssemblyDefinition systemAssembiy = AssemblyDefinition.ReadAssembly(System.Reflection.Assembly.Load(scAssembiy.MainModule.TypeSystem.Corlib.Name).GetFiles()[0]);

            //判断版本
            if (scAssembiy.Name.Version != new Version(scVersion))
            {
                System.Console.Out.Write("不支持的版本！仅支持" + scVersion + "\n");

                return -1;
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
                                                            FieldReference gameInitActionField = scAssembiy.MainModule.Import(field.Resolve());
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

                //获取玩家
                if (scClass.Name == "ComponentPlayer")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "Load")
                        {
                            ILProcessor ilProcessor = scMethod.Body.GetILProcessor().Body.GetILProcessor();
                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Ldarg_0));

                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "Player")
                                {
                                    foreach (MethodDefinition playerInitMethod in modType.Methods)
                                    {
                                        if (playerInitMethod.Name == "Initialize")
                                        {
                                            MethodReference playerInit = scAssembiy.MainModule.Import(playerInitMethod.Resolve());
                                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Call, playerInit));
                                            
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
                //获取地形
                if (scClass.Name == "SubsystemTerrain")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == ".ctor")
                        {
                            ILProcessor ilProcessor = scMethod.Body.GetILProcessor().Body.GetILProcessor();
                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Ldarg_0));

                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "Terrain")
                                {
                                    foreach (MethodDefinition terrainInitMethod in modType.Methods)
                                    {
                                        if (terrainInitMethod.Name == "Initialize")
                                        {
                                            MethodReference terrainInit = scAssembiy.MainModule.Import(terrainInitMethod.Resolve());
                                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Call, terrainInit));

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
                //获取实体管理器
                if (scClass.Name == "SubsystemEntityFactory")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == ".ctor")
                        {
                            ILProcessor ilProcessor = scMethod.Body.GetILProcessor().Body.GetILProcessor();
                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Ldarg_0));

                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "EntitySpawner")
                                {
                                    foreach (MethodDefinition terrainInitMethod in modType.Methods)
                                    {
                                        if (terrainInitMethod.Name == "Initialize")
                                        {
                                            MethodReference terrainInit = scAssembiy.MainModule.Import(terrainInitMethod.Resolve());
                                            ilProcessor.Body.Instructions.Insert(ilProcessor.Body.Instructions.Count - 1, Instruction.Create(OpCodes.Call, terrainInit));

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
                //地图监听
                if (scClass.Name == "SubsystemBlockBehaviors")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "Load")
                        {
                            //向其中增加BlockEvent
                            ILProcessor ilProcessor = scMethod.Body.GetILProcessor().Body.GetILProcessor();

                            //从114开始
                            int start = 114;

                            //读入列表
                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "BlockEvents")
                                {
                                    foreach (MethodDefinition blockEventInitMethod in modType.Methods)
                                    {
                                        if (blockEventInitMethod.Name == "Initialize")
                                        {
                                            ilProcessor.Body.Instructions.Insert(start + 0, Instruction.Create(OpCodes.Ldloc_0));
                                            MethodReference blockEventInit = scAssembiy.MainModule.Import(blockEventInitMethod.Resolve());
                                            ilProcessor.Body.Instructions.Insert(start + 1, Instruction.Create(OpCodes.Call, blockEventInit));
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

                //BlockManager的注入
                if (scClass.Name == "BlocksManager")
                {
                    foreach (MethodDefinition scMethod in scClass.Methods)
                    {
                        if (scMethod.Name == "Initialize")
                        {
                            ILProcessor ilProcessor = scMethod.Body.GetILProcessor().Body.GetILProcessor();

                            //修改循环范围
                            //移除代码
                            ilProcessor.Remove(scMethod.Body.Instructions[5]);
                            ilProcessor.Remove(scMethod.Body.Instructions[5]);
                            ilProcessor.Remove(scMethod.Body.Instructions[5]);
                            ilProcessor.Remove(scMethod.Body.Instructions[5]);

                            //寻找ModAPI中的变量
                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "BlocksManager")
                                {
                                    foreach (FieldDefinition field in modType.Fields)
                                        if (field.Name == "modBlocks")
                                        {
                                            FieldReference modBlockField = scAssembiy.MainModule.Import(field.Resolve());
                                            ilProcessor.Replace(scMethod.Body.Instructions[5], Instruction.Create(OpCodes.Ldsfld, modBlockField));
                                            break;
                                        }
                                    break;
                                }
                            }
                            //初始化ModAPI
                            foreach (TypeDefinition modType in modAPIAssembiy.MainModule.Types)
                            {
                                if (modType.Name == "BlocksManager")
                                {
                                    foreach (MethodDefinition modBlocksManagerInitMethod in modType.Methods)
                                    {
                                        if (modBlocksManagerInitMethod.Name == "Initialize")
                                        {
                                            MethodReference modBlocksManagerInit = scAssembiy.MainModule.Import(modBlocksManagerInitMethod.Resolve());
                                            ilProcessor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, modBlocksManagerInit));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            scAssembiy.Write(path.Substring(0, path.Length - 4) + "-ModAPI-" + modAPIVersion + ".exe");

            return 0;
        }
    }
}
