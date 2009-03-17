/// <copyright file="CatchVariableModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchVariableModel : ModelBase
    {
        private ICatchVariableDeclaration CatchVariableDeclaration { get; set; }

        private ICatchVariableDeclarationNode CatchVariableDeclarationNode
        {
            get { return this.CatchVariableDeclaration as ICatchVariableDeclarationNode; }
        }

        public ICSharpIdentifierNode VariableName
        {
            get { return this.CatchVariableDeclarationNode.Name; }
        }

        public CatchVariableModel(MethodDeclarationModel methodDeclarationModel, ICatchVariableDeclaration catchVariableDeclaration)
            : base(methodDeclarationModel)
        {
            CatchVariableDeclaration = catchVariableDeclaration;
        }
    }
}