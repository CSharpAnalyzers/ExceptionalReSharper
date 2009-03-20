/// <copyright file="MethodDeclarationModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    internal class MethodDeclarationModel : AnalyzeUnitModelBase<IMethodDeclarationNode>
    {
        public MethodDeclarationModel(IMethodDeclarationNode methodDeclaration)
            : base(null, methodDeclaration)
        {
            DocCommentBlockModel = new DocCommentBlockModel(this);
        }
    }
}