using System;
using System.Collections.Generic;

namespace RangHo.DialogueScript
{
	/// <summary>
    /// The interface that all host program that use DialogueScript should be inherited from.
    /// The interpreter will communicate with the host program using instance of this program.
    /// </summary>
    public interface IOutputManager
    {
        /// <summary>
        /// The interpreter will call this method every time it sees valid "Say Statement".
        /// </summary>
        /// <param name="speaker">It represents the speaker of the statement.</param>
        /// <param name="content">It represents the content of the statement.</param>
       	void Say(object speaker, string content);
        
        /// <summary>
        /// The interpreter will call this method to set the value of the selected property.
        /// </summary>
        /// <param name="value">The value of property.</param>
        void Set(object value);
        
        /// <summary>
        /// It registers an object to the dictionary.
        /// </summary>
        /// <param name="target">An instance of the object to register</param>
		/// <param name="name">Name of the object</param>
        void RegisterObject(object target, string name);
        
        /// <summary>
        /// This method should find an object with given name from registered object dictionary.
        /// </summary>
        /// <param name="name">Name of the object to find.</param>
        /// <returns>The object found. Null if it does not exist.</returns>
        object FindObject(string name);
        
        /// <summary>
        /// This method has to query whether the target is a property of given container object.
        /// </summary>
        /// <remarks>
        /// This method might have to utilize Reflection technology.
        /// It might slow down the program.
        /// </remarks>
        /// <param name="target">The name of target property to search.</param>
        /// <param name="container">Name of the container object which might contain the target.</param>
		/// <returns>True if the target object exists</returns>
        void Select(string target, string container = null);
        
        /// <summary>
        /// The interpreter will call this method when Exception occurs while interpreting.
        /// </summary>
        /// <param name="e">The <see cref="System.Exception" /> object that represents the exception occured.</param>
        void Exception(Exception e);
        
        /// <summary>
        /// The interpreter will call this method when choices are presented.
        /// </summary>
        /// <param name="ChoicesDictionary">Key represents the label, and Value represents the content.</param>
		/// <param name = "ChosenLabel">Change the value of this variable when users make choices.</param>
        void Choices(Dictionary<string, string> ChoicesDictionary, ref string ChosenLabel);
        
        /// <summary>
        /// The interpreter will call this method when it finishes reading the script.
        /// </summary>
        void Finish();
    }
}