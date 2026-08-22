using ModelosDB.General;
using ModelosDB.Inventario;
using WA_StoreControl.Services;

namespace WA_StoreControl.Tests
{
    public class FakeCategoriasService : CategoriasService
    {
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeCategoriasService(
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeUpdate(Categoria Categoria)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }

    public class FakeSubCategoriasService : SubCategoriasService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeSubCategoriasService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(SubCategoria SubCategoria)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(SubCategoria SubCategoria)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }

    public class FakeMarcasService : MarcasService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeMarcasService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(Marca Marca)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(Marca Marca)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }

    public class FakeTiposIdentificacionService : TiposIdentificacionService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeTiposIdentificacionService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(TipoIdentificacion TipoIdentificacion)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(TipoIdentificacion TipoIdentificacion)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }

    public class FakeCompaniasTelefonicaService : CompaniasTelefonicaService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeCompaniasTelefonicaService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(CompaniaTelefonica CompaniaTelefonica)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(CompaniaTelefonica CompaniaTelefonica)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }

    public class FakeProductosService : ProductosService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeProductosService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(Producto Producto)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(Producto Producto)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }
}
