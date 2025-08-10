using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosStarProcessor
{
    public class StarDataFull
    {
        public string StarName { get; set; }
        /// <summary>
        /// [H] Catalogue (H=Hipparcos)
        /// Каталог (H=Hipparcos)
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Identifier (HIP number)
        /// Идентификатор (номер HIP)
        /// </summary>
        public int HIP { get; set; }

        /// <summary>
        /// *[HT] Proximity flag
        /// Флаг близости
        /// </summary>
        public string Proxy { get; set; }

        /// <summary>
        /// Right ascension in h m s, ICRS (J1991.25)
        /// Прямое восхождение в часах минутах секундах, ICRS (J1991.25)
        /// </summary>
        public string RAhms { get; set; }

        /// <summary>
        /// Declination in deg ' ", ICRS (J1991.25)
        /// Склонение в градусах минутах секундах, ICRS (J1991.25)
        /// </summary>
        public string DEdms { get; set; }

        /// <summary>
        /// ? Magnitude in Johnson V
        /// Величина в системе Джонсона V
        /// </summary>
        public double? Vmag { get; set; }

        /// <summary>
        /// *[1,3]? Coarse variability flag
        /// Грубая переменность флага
        /// </summary>
        public int? VarFlag { get; set; }

        /// <summary>
        /// *[GHT] Source of magnitude
        /// Источник величины
        /// </summary>
        public string r_Vmag { get; set; }

        /// <summary>
        /// *? alpha, degrees (ICRS, Epoch=J1991.25)
        /// Альфа, градусы (ICRS, эпоха = J1991.25)
        /// </summary>
        public double? RAdeg { get; set; }

        /// <summary>
        /// *? delta, degrees (ICRS, Epoch=J1991.25)
        /// Дельта, градусы (ICRS, эпоха = J1991.25)
        /// </summary>
        public double? DEdeg { get; set; }

        /// <summary>
        /// *[*+A-Z] Reference flag for astrometry
        /// Флаг ссылки для астрометрии
        /// </summary>
        public string AstroRef { get; set; }

        /// <summary>
        /// ? Trigonometric parallax
        /// Тригонометрический параллакс
        /// </summary>
        public double? Plx { get; set; }

        /// <summary>
        /// *? Proper motion mu_alpha.cos(delta), ICRS
        /// Собственное движение mu_alpha.cos(delta), ICRS
        /// </summary>
        public double? pmRA { get; set; }

        /// <summary>
        /// *? Proper motion mu_delta, ICRS
        /// Собственное движение mu_delta, ICRS
        /// </summary>
        public double? pmDE { get; set; }

        /// <summary>
        /// *? Standard error in RA*cos(DEdeg)
        /// Стандартная ошибка в RA*cos(DEdeg)
        /// </summary>
        public double? e_RAdeg { get; set; }

        /// <summary>
        /// *? Standard error in DE
        /// Стандартная ошибка в DE
        /// </summary>
        public double? e_DEdeg { get; set; }

        /// <summary>
        /// ? Standard error in Plx
        /// Стандартная ошибка в Plx
        /// </summary>
        public double? e_Plx { get; set; }

        /// <summary>
        /// ? Standard error in pmRA
        /// Стандартная ошибка в pmRA
        /// </summary>
        public double? e_pmRA { get; set; }

        /// <summary>
        /// ? Standard error in pmDE
        /// Стандартная ошибка в pmDE
        /// </summary>
        public double? e_pmDE { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, DE/RA*cos(delta)
        /// Корреляция, DE/RA*cos(delta)
        /// </summary>
        public double? DE_RA { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, Plx/RA*cos(delta)
        /// Корреляция, Plx/RA*cos(delta)
        /// </summary>
        public double? Plx_RA { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, Plx/DE
        /// Корреляция, Plx/DE
        /// </summary>
        public double? Plx_DE { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmRA/RA*cos(delta)
        /// Корреляция, pmRA/RA*cos(delta)
        /// </summary>
        public double? pmRA_RA { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmRA/DE
        /// Корреляция, pmRA/DE
        /// </summary>
        public double? pmRA_DE { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmRA/Plx
        /// Корреляция, pmRA/Plx
        /// </summary>
        public double? pmRA_Plx { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmDE/RA*cos(delta)
        /// Корреляция, pmDE/RA*cos(delta)
        /// </summary>
        public double? pmDE_RA { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmDE/DE
        /// Корреляция, pmDE/DE
        /// </summary>
        public double? pmDE_DE { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmDE/Plx
        /// Корреляция, pmDE/Plx
        /// </summary>
        public double? pmDE_Plx { get; set; }

        /// <summary>
        /// [-1/1]? Correlation, pmDE/pmRA
        /// Корреляция, pmDE/pmRA
        /// </summary>
        public double? pmDE_pmRA { get; set; }

        /// <summary>
        /// ? Percentage of rejected data
        /// Процент отклоненных данных
        /// </summary>
        public int? F1 { get; set; }

        /// <summary>
        /// *? Goodness-of-fit parameter
        /// Параметр соответствия
        /// </summary>
        public double? F2 { get; set; }

        /// <summary>
        /// HIP number (repetition)
        /// Номер HIP (повторение)
        /// </summary>
        public int? HIP_Repetition { get; set; }

        /// <summary>
        /// ? Mean BT magnitude
        /// Средняя величина BT
        /// </summary>
        public double? BTmag { get; set; }

        /// <summary>
        /// ? Standard error on BTmag
        /// Стандартная ошибка на BTmag
        /// </summary>
        public double? e_BTmag { get; set; }

        /// <summary>
        /// ? Mean VT magnitude
        /// Средняя величина VT
        /// </summary>
        public double? VTmag { get; set; }

        /// <summary>
        /// ? Standard error on VTmag
        /// Стандартная ошибка на VTmag
        /// </summary>
        public double? e_VTmag { get; set; }

        /// <summary>
        /// *[A-Z*-] Reference flag for BT and VTmag
        /// Флаг ссылки для BT и VTmag
        /// </summary>
        public string m_BTmag { get; set; }

        /// <summary>
        /// ? Johnson B-V colour
        /// Цветовой индекс B-V системы Джонсона
        /// </summary>
        public double? B_V { get; set; }

        /// <summary>
        /// ? Standard error on B-V
        /// Стандартная ошибка на B-V
        /// </summary>
        public double? e_B_V { get; set; }

        /// <summary>
        /// [GT] Source of B-V from Ground or Tycho
        /// Источник B-V от Земли или Тихо
        /// </summary>
        public string r_B_V { get; set; }

        /// <summary>
        /// ? Colour index in Cousins' system
        /// Индекс цвета в системе Казинс
        /// </summary>
        public double? V_I { get; set; }

        /// <summary>
        /// ? Standard error on V-I
        /// Стандартная ошибка на V-I
        /// </summary>
        public double? e_V_I { get; set; }

        /// <summary>
        /// *[A-T] Source of V-I
        /// Источник V-I
        /// </summary>
        public string r_V_I { get; set; }

        /// <summary>
        /// Флаг для комбинированной величины Vmag, B-V, V-I
        /// </summary>
        public string CombMag { get; set; }

        /// <summary>
        /// Медиана величины в системе Гиппаркос
        /// </summary>
        public double? Hpmag { get; set; }

        /// <summary>
        /// Стандартная ошибка на Hpmag
        /// </summary>
        public double? E_Hpmag { get; set; }

        /// <summary>
        /// Разброс на Hpmag
        /// </summary>
        public double? Hpscat { get; set; }

        /// <summary>
        /// Количество наблюдений для Hpmag
        /// </summary>
        public int? O_Hpmag { get; set; }

        /// <summary>
        /// Флаг ссылки для Hpmag
        /// </summary>
        public string M_Hpmag { get; set; }

        /// <summary>
        /// Величина Hpmag при максимуме (5-й процентиль)
        /// </summary>
        public double? Hpmax { get; set; }

        /// <summary>
        /// Величина Hpmag при минимуме (95-й процентиль)
        /// </summary>
        public double? HPmin { get; set; }

        /// <summary>
        /// Период переменности (дни)
        /// </summary>
        public double? Period { get; set; }

        /// <summary>
        /// Тип переменности
        /// </summary>
        public string HvarType { get; set; }

        /// <summary>
        /// Дополнительные данные о переменности
        /// </summary>
        public string MoreVar { get; set; }

        /// <summary>
        /// Приложение световой кривой
        /// </summary>
        public string MorePhoto { get; set; }

        /// <summary>
        /// Идентификатор CCDM
        /// </summary>
        public string CCDM { get; set; }

        /// <summary>
        /// Исторический статус флаг
        /// </summary>
        public string N_CCDM { get; set; }

        /// <summary>
        /// Количество записей с тем же CCDM
        /// </summary>
        public int? Nsys { get; set; }

        /// <summary>
        /// Количество компонентов в этой записи
        /// </summary>
        public int? Ncomp { get; set; }

        /// <summary>
        /// Флаг двойной/множественной системы
        /// </summary>
        public string MultFlag { get; set; }

        /// <summary>
        /// Флаг астрометрического источника
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Качество решения
        /// </summary>
        public string Qual { get; set; }

        /// <summary>
        /// Идентификаторы компонента
        /// </summary>
        public string M_HIP { get; set; }

        /// <summary>
        /// Угол между компонентами
        /// </summary>
        public int? Theta { get; set; }

        /// <summary>
        /// Угловое разделение между компонентами
        /// </summary>
        public double? Rho { get; set; }

        /// <summary>
        /// Стандартная ошибка на rho
        /// </summary>
        public double? E_Rho { get; set; }

        /// <summary>
        /// Разница величин компонентов
        /// </summary>
        public double? DHp { get; set; }

        /// <summary>
        /// Стандартная ошибка на dHp
        /// </summary>
        public double? E_DHp { get; set; }

        /// <summary>
        /// Флаг, указывающий на звезду обзора
        /// </summary>
        public string Survey { get; set; }

        /// <summary>
        /// Идентификационная диаграмма
        /// </summary>
        public string Chart { get; set; }

        /// <summary>
        /// Наличие примечаний
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Номер HD
        /// </summary>
        public int? HD { get; set; }

        /// <summary>
        /// Боннерская звездная карта (BD)
        /// </summary>
        public string BD { get; set; }

        /// <summary>
        /// Кордовский обзор (CoD)
        /// </summary>
        public string CoD { get; set; }

        /// <summary>
        /// Кейптаунский фотографический обзор (CPD)
        /// </summary>
        public string CPD { get; set; }

        /// <summary>
        /// V-I, используемый для сокращений
        /// </summary>
        public double? VIred { get; set; }

        /// <summary>
        /// Спектральный тип
        /// </summary>
        public string SpType { get; set; }

        /// <summary>
        /// Источник спектрального типа
        /// </summary>
        public string r_SpType { get; set; }

    }
}
