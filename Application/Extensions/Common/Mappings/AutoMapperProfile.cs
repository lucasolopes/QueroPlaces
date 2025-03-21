using Application.Extensions.Common.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Extensions.Extensions.Common.Mappings;

/// <summary>
///     Perfil de mapeamento para AutoMapper
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapeamentos de Bairro
        CreateMap<Bairro, BairroDTO>()
            .ForMember(dest => dest.LocalidadeNome, opt => opt.MapFrom(src => src.Localidade.Nome));

        // Mapeamentos de Localidade
        CreateMap<Localidade, LocalidadeDTO>()
            .ForMember(dest => dest.TipoLocalidade, opt => opt.MapFrom(src => MapearTipoLocalidade(src.TipoLocalidade)));

        // Mapeamentos de Logradouro
        CreateMap<Logradouro, LogradouroDTO>()
            .ForMember(dest => dest.LocalidadeNome, opt => opt.MapFrom(src => src.Localidade.Nome))
            .ForMember(dest => dest.BairroNome, opt => opt.MapFrom(src => src.BairroInicial.Nome));

        // Mapeamentos complexos com composição de várias entidades
        CreateMap<Logradouro, EnderecoDTO>()
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.BairroInicial.Nome))
            .ForMember(dest => dest.Localidade, opt => opt.MapFrom(src => src.Localidade.Nome))
            .ForMember(dest => dest.TipoCEP, opt => opt.MapFrom(src => "LOGRADOURO"));

        CreateMap<Localidade, EnderecoDTO>()
            .ForMember(dest => dest.Localidade, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.TipoCEP, opt => opt.MapFrom(src => "LOCALIDADE"));

        CreateMap<GrandeUsuario, EnderecoDTO>()
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro.Nome))
            .ForMember(dest => dest.Localidade, opt => opt.MapFrom(src => src.Localidade.Nome))
            .ForMember(dest => dest.TipoCEP, opt => opt.MapFrom(src => "GRANDE_USUARIO"));

        CreateMap<UnidadeOperacional, EnderecoDTO>()
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Bairro.Nome))
            .ForMember(dest => dest.Localidade, opt => opt.MapFrom(src => src.Localidade.Nome))
            .ForMember(dest => dest.TipoCEP, opt => opt.MapFrom(src => "UNIDADE_OPERACIONAL"));

        CreateMap<CaixaPostalComunitaria, EnderecoDTO>()
            .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Localidade, opt => opt.MapFrom(src => src.Localidade.Nome))
            .ForMember(dest => dest.TipoCEP, opt => opt.MapFrom(src => "CAIXA_POSTAL_COMUNITARIA"));
    }

    private string MapearTipoLocalidade(string tipoLocalidade)
    {
        return tipoLocalidade switch
        {
            "C" => "Cidade",
            "M" => "Município",
            "D" => "Distrito",
            "P" => "Povoado",
            "A" => "Área",
            _ => tipoLocalidade
        };
    }
}