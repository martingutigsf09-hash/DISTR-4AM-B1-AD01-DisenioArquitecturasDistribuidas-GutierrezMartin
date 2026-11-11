
import { useState } from "react";
import {
  LayoutDashboard, Package, Warehouse, ShoppingCart, Tags,
  Users, Search, Bell, Plus, ArrowUpRight, TrendingUp,
  CircleDollarSign, Boxes, MoreHorizontal, Menu, X,
  Download, CalendarDays, ChevronDown, Activity,
  Settings, LogOut, Truck, Clock, CheckCircle2,
  AlertTriangle, SlidersHorizontal
} from "lucide-react";

const menu = [
  { title: "GENERAL", items: [
    { name: "Dashboard", icon: LayoutDashboard },
    { name: "Productos", icon: Package },
    { name: "Inventario", icon: Warehouse },
    { name: "Pedidos", icon: ShoppingCart }
  ]},
  { title: "ADMINISTRACIÓN", items: [
    { name: "Categorías", icon: Tags },
    { name: "Clientes", icon: Users }
  ]}
];

const initialProducts = [
  { id: 1, name: "Máquina Cheyenne SOL Nova", sku: "CH-SOL-001", category: "Máquinas", price: 450, stock: 20 },
  { id: 2, name: "Tinta Dynamic Black", sku: "DT-BLK-002", category: "Tintas", price: 35, stock: 8 },
  { id: 3, name: "Agujas RL 1205", sku: "AG-RL-003", category: "Agujas", price: 12, stock: 3 },
  { id: 4, name: "Fuente de poder digital", sku: "FP-DIG-004", category: "Accesorios", price: 85, stock: 0 },
  { id: 5, name: "Grip profesional", sku: "GR-PRO-005", category: "Accesorios", price: 28, stock: 15 }
];

const initialOrders = [
  { id: 5, client: "Juan Pérez", date: "11 Nov 2026", total: 1000, status: "Pendiente" },
  { id: 4, client: "María López", date: "10 Nov 2026", total: 500, status: "Completado" },
  { id: 3, client: "Carlos Ruiz", date: "09 Nov 2026", total: 250, status: "Completado" },
  { id: 2, client: "Ana Torres", date: "08 Nov 2026", total: 120, status: "Pendiente" }
];

const clients = [
  { name: "Juan Pérez", email: "juan.perez@gmail.com", phone: "0991234567" },
  { name: "María López", email: "maria.lopez@gmail.com", phone: "0987654321" },
  { name: "Carlos Ruiz", email: "carlos.ruiz@gmail.com", phone: "0971234567" }
];

const categories = ["Máquinas", "Tintas", "Agujas", "Accesorios"];

const money = n => "$" + n.toLocaleString("en-US", {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2
});

function App() {
  const [active, setActive] = useState("Dashboard");
  const [search, setSearch] = useState("");
  const [mobileOpen, setMobileOpen] = useState(false);
  const [products, setProducts] = useState(initialProducts);
  const [orders] = useState(initialOrders);
  const [filter, setFilter] = useState("Todos");

  const goTo = name => {
    setActive(name);
    setSearch("");
    setMobileOpen(false);
    setFilter("Todos");
  };

  const filteredProducts = products.filter(p => {
    const matches = `${p.name} ${p.sku} ${p.category}`.toLowerCase()
      .includes(search.toLowerCase());
    const matchesFilter = filter === "Todos" ||
      (filter === "Stock bajo" && p.stock > 0 && p.stock <= 5) ||
      (filter === "Agotados" && p.stock === 0);
    return matches && matchesFilter;
  });

  const filteredOrders = orders.filter(o =>
    `${o.id} ${o.client} ${o.status}`.toLowerCase().includes(search.toLowerCase())
  );

  const pageTitles = {
    Dashboard: "Panel general",
    Productos: "Gestión de productos",
    Inventario: "Control de inventario",
    Pedidos: "Gestión de pedidos",
    Categorías: "Categorías",
    Clientes: "Directorio de clientes"
  };

  const addProduct = () => {
    const name = prompt("Nombre del nuevo producto:");
    if (!name || !name.trim()) return;
    const price = Number(prompt("Precio del producto:", "25"));
    if (!Number.isFinite(price) || price < 0) {
      alert("Introduce un precio válido.");
      return;
    }
    setProducts(prev => [...prev, {
      id: Date.now(),
      name: name.trim(),
      sku: "NUE-" + Date.now().toString().slice(-5),
      category: "Accesorios",
      price,
      stock: 0
    }]);
  };

  return (
    <div className="app">
      {mobileOpen && <div className="overlay" onClick={() => setMobileOpen(false)} />}

      <aside className={`sidebar ${mobileOpen ? "sidebar-open" : ""}`}>
        <div className="brand">
          <div className="brand-mark">A<span>.</span></div>
          <div className="brand-copy">
            <strong>ARQ<span>DIST</span></strong>
            <small>MANAGEMENT SYSTEM</small>
          </div>
          <button className="mobile-close" onClick={() => setMobileOpen(false)}><X/></button>
        </div>

        <div className="workspace">
          <div className="workspace-icon">AD</div>
          <div className="workspace-copy">
            <strong>Administración</strong>
            <small>Espacio de trabajo</small>
          </div>
          <ChevronDown size={16}/>
        </div>

        <nav>
          {menu.map(group => (
            <div className="nav-group" key={group.title}>
              <p className="nav-heading">{group.title}</p>
              {group.items.map(item => {
                const Icon = item.icon;
                return (
                  <button
                    key={item.name}
                    onClick={() => goTo(item.name)}
                    className={`nav-link ${active === item.name ? "active" : ""}`}
                  >
                    <Icon size={19}/>
                    <span>{item.name}</span>
                    {item.name === "Pedidos" && <small className="nav-badge">4</small>}
                  </button>
                );
              })}
            </div>
          ))}
        </nav>

        <div className="sidebar-bottom">
          <div className="connection"><i/> Servicios disponibles</div>
          <div className="user-profile">
            <div className="user-avatar">MG</div>
            <div className="user-info">
              <strong>Administrador</strong>
              <small>Gestor del sistema</small>
            </div>
            <MoreHorizontal size={20}/>
          </div>
        </div>
      </aside>

      <main className="main">
        <header className="topbar">
          <button className="menu-toggle" onClick={() => setMobileOpen(true)}><Menu/></button>
          <div className="breadcrumbs">Sistema <span>/</span> <strong>{active}</strong></div>
          <div className="topbar-actions">
            <label className="search">
              <Search size={17}/>
              <input
                value={search}
                onChange={e => setSearch(e.target.value)}
                placeholder="Buscar..."
              />
            </label>
            <button className="notification"><Bell size={19}/><i/></button>
            <div className="top-user">MG</div>
          </div>
        </header>

        <div className="content">
          <div className="page-header">
            <div>
              <span className="overline">ARQUITECTURA DISTRIBUIDA</span>
              <h1>{pageTitles[active]}</h1>
              <p>Administra y supervisa las operaciones de tu sistema.</p>
            </div>
            {active === "Dashboard" && (
              <button className="secondary-button"><Download size={16}/> Exportar</button>
            )}
          </div>

          {active === "Dashboard" && (
            <>
              <div className="welcome-row">
                <div>
                  <span className="muted-label">RESUMEN GENERAL</span>
                  <h2>¡Bienvenido de nuevo!</h2>
                  <p>Esto es lo que está pasando en tu negocio hoy.</p>
                </div>
                <div className="date-label"><CalendarDays size={17}/> 11 de noviembre, 2026</div>
              </div>

              <div className="stats">
                <Stat icon={Package} label="Total productos" value={products.length} change="+12%" />
                <Stat icon={ShoppingCart} label="Pedidos registrados" value="48" change="+8%" />
                <Stat icon={CircleDollarSign} label="Ventas registradas" value="$12,450" change="+15%" />
                <Stat icon={Warehouse} label="Unidades en stock" value={products.reduce((a,p)=>a+p.stock,0)} change="+5%" />
              </div>

              <div className="dashboard-grid">
                <section className="panel">
                  <div className="panel-title">
                    <div><h3>Actividad de ventas</h3><p>Resumen de los últimos siete días</p></div>
                    <span className="panel-tag">Esta semana <ChevronDown size={14}/></span>
                  </div>
                  <div className="sales-total"><strong>$12,450.00</strong><span><TrendingUp size={14}/> 15.8%</span></div>
                  <div className="chart">
                    {[35,55,43,75,52,90,68].map((n,i)=>(
                      <div className="chart-item" key={i}>
                        <div className="chart-track">
                          <div className={`chart-bar ${i===5?"highlight":""}`} style={{height:`${n}%`}}/>
                        </div>
                        <small>{["Lun","Mar","Mié","Jue","Vie","Sáb","Dom"][i]}</small>
                      </div>
                    ))}
                  </div>
                </section>

                <section className="panel">
                  <div className="panel-title">
                    <div><h3>Estado del inventario</h3><p>Disponibilidad de productos</p></div>
                    <Boxes className="orange-icon"/>
                  </div>
                  <div className="inventory-total">{products.length}<small> productos registrados</small></div>
                  <div className="inventory-bar">
                    <span className="available" style={{width:`${products.filter(p=>p.stock>5).length/products.length*100}%`}}/>
                    <span className="low" style={{width:`${products.filter(p=>p.stock>0&&p.stock<=5).length/products.length*100}%`}}/>
                    <span className="empty" style={{width:`${products.filter(p=>p.stock===0).length/products.length*100}%`}}/>
                  </div>
                  <div className="legend">
                    <p><i className="green-dot"/> Disponible <b>{products.filter(p=>p.stock>5).length}</b></p>
                    <p><i className="orange-dot"/> Stock bajo <b>{products.filter(p=>p.stock>0&&p.stock<=5).length}</b></p>
                    <p><i className="red-dot"/> Agotado <b>{products.filter(p=>p.stock===0).length}</b></p>
                  </div>
                </section>
              </div>

              <section className="panel table-panel">
                <div className="panel-title">
                  <div><h3>Pedidos recientes</h3><p>Últimas operaciones registradas</p></div>
                  <button className="link-button" onClick={()=>goTo("Pedidos")}>Ver todos <ArrowUpRight size={16}/></button>
                </div>
                <OrderTable data={orders.slice(0,3)}/>
              </section>
            </>
          )}

          {(active === "Productos" || active === "Inventario") && (
            <section className="panel table-panel">
              <div className="panel-title">
                <div>
                  <h3>{active === "Productos" ? "Listado de productos" : "Existencias actuales"}</h3>
                  <p>{active === "Productos" ? "Administra los productos de tu catálogo." : "Consulta las cantidades disponibles."}</p>
                </div>
                {active === "Productos" ?
                  <button className="primary-button" onClick={addProduct}><Plus size={17}/> Nuevo producto</button> :
                  <button className="secondary-button"><Download size={16}/> Exportar</button>}
              </div>
              <div className="filters">
                <SlidersHorizontal size={16}/>
                {["Todos","Stock bajo","Agotados"].map(f=>(
                  <button key={f} className={filter===f?"filter-active":""} onClick={()=>setFilter(f)}>{f}</button>
                ))}
              </div>
              <ProductTable data={filteredProducts}/>
            </section>
          )}

          {active === "Pedidos" && (
            <section className="panel table-panel">
              <div className="panel-title">
                <div><h3>Registro de pedidos</h3><p>Consulta el historial de operaciones.</p></div>
                <button className="secondary-button"><Download size={16}/> Exportar</button>
              </div>
              <OrderTable data={filteredOrders}/>
            </section>
          )}

          {active === "Clientes" && (
            <section className="panel table-panel">
              <div className="panel-title"><div><h3>Directorio de clientes</h3><p>Clientes registrados en el sistema.</p></div></div>
              <div className="table-wrap"><table>
                <thead><tr><th>CLIENTE</th><th>CORREO</th><th>TELÉFONO</th></tr></thead>
                <tbody>{clients.filter(c=>`${c.name} ${c.email}`.toLowerCase().includes(search.toLowerCase())).map(c=>(
                  <tr key={c.email}><td><b>{c.name}</b></td><td>{c.email}</td><td>{c.phone}</td></tr>
                ))}</tbody>
              </table></div>
            </section>
          )}

          {active === "Categorías" && (
            <section className="panel category-grid">
              {categories.filter(c=>c.toLowerCase().includes(search.toLowerCase())).map((c,i)=>(
                <div className="category-card" key={c}><div className="category-icon"><Tags/></div><h3>{c}</h3><p>Categoría de catálogo</p><span>0{i+1}</span></div>
              ))}
            </section>
          )}

          <footer>© 2026 ARQDIST · Sistema de gestión distribuida</footer>
        </div>
      </main>
    </div>
  );
}

function Stat({icon:Icon,label,value,change}) {
  return (
    <div className="stat-card">
      <div className="stat-head"><div className="stat-icon"><Icon size={20}/></div><span className="growth">{change} <ArrowUpRight size={13}/></span></div>
      <strong className="stat-value">{value}</strong>
      <span className="stat-label">{label}</span>
    </div>
  );
}

function ProductTable({data}) {
  return (
    <div className="table-wrap"><table>
      <thead><tr><th>PRODUCTO</th><th>SKU</th><th>CATEGORÍA</th><th>PRECIO</th><th>STOCK</th><th>ESTADO</th></tr></thead>
      <tbody>
        {data.map(p=>(
          <tr key={p.id}>
            <td><div className="product-name"><div className="product-thumb"><Package size={17}/></div><b>{p.name}</b></div></td>
            <td className="muted">{p.sku}</td><td>{p.category}</td><td><b>{money(p.price)}</b></td><td>{p.stock} unidades</td>
            <td><Status value={p.stock===0?"Agotado":p.stock<=5?"Stock bajo":"Disponible"}/></td>
          </tr>
        ))}
        {data.length===0&&<tr><td colSpan="6" className="no-data">No se encontraron resultados.</td></tr>}
      </tbody>
    </table></div>
  );
}

function OrderTable({data}) {
  return (
    <div className="table-wrap"><table>
      <thead><tr><th>PEDIDO</th><th>CLIENTE</th><th>FECHA</th><th>TOTAL</th><th>ESTADO</th></tr></thead>
      <tbody>
        {data.map(o=>(
          <tr key={o.id}><td><b>#PED-{String(o.id).padStart(3,"0")}</b></td><td>{o.client}</td><td className="muted">{o.date}</td><td><b>{money(o.total)}</b></td><td><Status value={o.status}/></td></tr>
        ))}
        {data.length===0&&<tr><td colSpan="5" className="no-data">No hay pedidos registrados.</td></tr>}
      </tbody>
    </table></div>
  );
}

function Status({value}) {
  const type = value==="Disponible"||value==="Completado"?"success":value==="Agotado"?"danger":"warning";
  return <span className={`status ${type}`}><i/>{value}</span>;
}

export default App;
