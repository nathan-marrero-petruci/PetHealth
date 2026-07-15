import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { removeToken } from "../auth/token";
import { NAVEGACAO } from "../navigation";

function agruparPorModulo(itens) {
  const grupos = [];
  const indiceDoGrupo = {};

  itens.forEach((item) => {
    if (!(item.modulo in indiceDoGrupo)) {
      indiceDoGrupo[item.modulo] = grupos.length;
      grupos.push({ modulo: item.modulo, itens: [] });
    }

    grupos[indiceDoGrupo[item.modulo]].itens.push(item);
  });

  return grupos;
}

const GRUPOS_NAVEGACAO = agruparPorModulo(NAVEGACAO);

export function AppLayout() {
  const [menuAberto, setMenuAberto] = useState(false);
  const navigate = useNavigate();

  function handleLogout() {
    removeToken();
    navigate("/login");
  }

  return (
    <div className="app-layout">
      <button
        type="button"
        className="app-menu-toggle"
        onClick={() => setMenuAberto((aberto) => !aberto)}
      >
        Menu
      </button>

      <nav className={menuAberto ? "app-nav app-nav-aberto" : "app-nav"}>
        {GRUPOS_NAVEGACAO.map((grupo) => (
          <div key={grupo.modulo} className="app-nav-grupo">
            <p className="app-nav-grupo-titulo">{grupo.modulo}</p>
            <ul>
              {grupo.itens.map((item) => (
                <li key={item.rota}>
                  <NavLink
                    to={item.rota}
                    className={({ isActive }) =>
                      isActive ? "app-nav-link app-nav-link-ativo" : "app-nav-link"
                    }
                    onClick={() => setMenuAberto(false)}
                  >
                    {item.label}
                  </NavLink>
                </li>
              ))}
            </ul>
          </div>
        ))}

        <button type="button" className="app-nav-logout" onClick={handleLogout}>
          Sair
        </button>
      </nav>

      <main className="app-content">
        <Outlet />
      </main>
    </div>
  );
}
