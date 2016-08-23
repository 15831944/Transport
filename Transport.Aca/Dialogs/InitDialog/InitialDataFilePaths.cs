using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Transport.Aca.Dialogs.InitDialog
{
    public class InitialDataFilePaths : IDataErrorInfo
    {
        public string DeparturesFilePath { get; set; }

        public string ArrivalsFilePath { get; set; }

        public string AdjacencyMatrixFilePath { get; set; }

        public string DirectTravelersMatrixFilePath { get; set; }
        public string NodesPositionsFilePath { get; set; }

        public bool HasDirectTravelersMatrix { get; set; }

        #region Validation

        string IDataErrorInfo.this[string propertyName] => GetValidationError(propertyName);

        string IDataErrorInfo.Error => null;


        public bool IsValid => ValidatedProperties.All(validatedProperty => GetValidationError(validatedProperty) == null);

        private static readonly string[] ValidatedProperties =
        {
            nameof(AdjacencyMatrixFilePath),
            nameof(DirectTravelersMatrixFilePath),
            nameof(DeparturesFilePath),
            nameof(ArrivalsFilePath),
            nameof(NodesPositionsFilePath)
        };

        private string GetValidationError(string propertyName)
        {
            string error = null;

            if (propertyName == nameof(AdjacencyMatrixFilePath))
            {
                error = ValidatePath(AdjacencyMatrixFilePath);
            }
            else if (propertyName == nameof(NodesPositionsFilePath))
            {
                error = ValidatePath(NodesPositionsFilePath);
            }

            if (HasDirectTravelersMatrix)
            {
                if (propertyName == nameof(DirectTravelersMatrixFilePath))
                {
                    error = ValidatePath(DirectTravelersMatrixFilePath);
                }
            }
            else
            {
                if (propertyName == nameof(ArrivalsFilePath))
                {
                    error = ValidatePath(ArrivalsFilePath);
                }
                if (propertyName == nameof(DeparturesFilePath))
                {
                    error = ValidatePath(DeparturesFilePath);
                }
            }

            return error;
        }

        private string ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return @"Путь не может быть пустым";
            }
            if (File.Exists(path) == false)
            {
                return @"Файл не существует";
            }

            return null;
        }

        #endregion
    }
}
