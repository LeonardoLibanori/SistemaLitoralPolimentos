/**
 * photon-autocomplete.js
 * Autocomplete de endereço via proxy server-side (/Home/BuscarEndereco).
 * Uso: photonAutocomplete('idDoCampo')
 */
(function () {
    'use strict';

    const CSS_ID = 'photon-ac-style';
    if (!document.getElementById(CSS_ID)) {
        const s = document.createElement('style');
        s.id = CSS_ID;
        s.textContent = `
        .ph-wrap { position: relative; }
        .ph-lista {
            position: absolute; top: calc(100% + 4px); left: 0; right: 0;
            background: #fff; border: 1px solid #e2e8f0; border-radius: 10px;
            box-shadow: 0 8px 24px rgba(0,0,0,.12); z-index: 9999;
            max-height: 260px; overflow-y: auto; padding: 4px 0;
            animation: phFade .15s ease;
        }
        @keyframes phFade { from{opacity:0;transform:translateY(-4px)} to{opacity:1;transform:translateY(0)} }
        .ph-item {
            display: flex; align-items: flex-start; gap: 10px;
            padding: 10px 14px; cursor: pointer; font-size: 13.5px;
            color: #334155; border-bottom: 1px solid #f8fafc; transition: background .1s;
        }
        .ph-item:last-child { border-bottom: none; }
        .ph-item:hover, .ph-item.sel { background: #f1f5f9; }
        .ph-ico  { margin-top:2px; flex-shrink:0; opacity:.4; }
        .ph-main { font-weight:600; color:#0f172a; font-size:13.5px; }
        .ph-sub  { font-size:12px; color:#64748b; margin-top:2px; }
        .ph-msg  { padding:12px 14px; font-size:13px; color:#94a3b8;
                   display:flex; align-items:center; gap:8px; }
        .ph-spin { width:13px; height:13px; border:2px solid #e2e8f0;
                   border-top-color:#64748b; border-radius:50%;
                   animation:phSpin .6s linear infinite; flex-shrink:0; }
        @keyframes phSpin { to{transform:rotate(360deg)} }
        `;
        document.head.appendChild(s);
    }

    const ICO_PIN = `<svg class="ph-ico" width="14" height="14" viewBox="0 0 24 24"
        fill="none" stroke="currentColor" stroke-width="2.2"
        stroke-linecap="round" stroke-linejoin="round">
        <path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0 1 18 0z"/>
        <circle cx="12" cy="10" r="3"/></svg>`;

    function montar(p) {
        const partes = [];
        const rua = p.street || '';
        const num = p.housenumber || '';
        if (rua) partes.push(num ? `${rua}, ${num}` : rua);
        else if (p.name && p.osm_type !== 'R') partes.push(p.name);
        if (p.district || p.suburb) partes.push(p.district || p.suburb);
        const cidade = p.city || p.town || p.village || p.municipality || '';
        if (cidade) partes.push(cidade);
        if (p.state) partes.push(p.state);
        return partes.join(', ');
    }

    function display(p) {
        const rua = p.street || p.name || '';
        const num = p.housenumber || '';
        const cidade = p.city || p.town || p.village || p.municipality || '';
        const bairro = p.district || p.suburb || '';
        const estado = p.state || '';
        const principal = rua ? (num ? `${rua}, ${num}` : rua) : cidade;
        return { principal, sub: [bairro, cidade, estado].filter(Boolean).join(', ') };
    }

    window.photonAutocomplete = function (inputId) {
        const input = document.getElementById(inputId);
        if (!input) return;

        // Evita dupla inicialização
        if (input.dataset.photonIniciado) return;
        input.dataset.photonIniciado = '1';

        const wrap = document.createElement('div');
        wrap.className = 'ph-wrap';
        input.parentNode.insertBefore(wrap, input);
        wrap.appendChild(input);

        let lista = null;
        let timer = null;
        let idxSel = -1;
        let ctrl = null;

        function abrir() {
            if (lista) return;
            lista = document.createElement('div');
            lista.className = 'ph-lista';
            wrap.appendChild(lista);
            idxSel = -1;
        }
        function fechar() {
            if (lista) { lista.remove(); lista = null; }
            idxSel = -1;
        }
        function els() { return lista ? Array.from(lista.querySelectorAll('.ph-item')) : []; }
        function realcar(i) { els().forEach((e, j) => e.classList.toggle('sel', i === j)); }

        async function buscar(q) {
            if (ctrl) ctrl.abort();
            ctrl = new AbortController();
            abrir();
            lista.innerHTML = `<div class="ph-msg"><div class="ph-spin"></div>Buscando...</div>`;

            try {
                // Chama o proxy server-side — mesmo domínio, sem CORS
                const url = `/Home/BuscarEndereco?q=${encodeURIComponent(q)}`;
                const r = await fetch(url, { signal: ctrl.signal });
                const j = await r.json();

                const features = (j.features || []).filter(f => {
                    const cc = (f.properties.country_code || '').toLowerCase();
                    const cn = (f.properties.country || '').toLowerCase();
                    return cc === 'br' || cn.includes('brasil') || cn.includes('brazil');
                });

                if (!features.length) {
                    lista.innerHTML = `<div class="ph-msg">Nenhum endereço encontrado.</div>`;
                    return;
                }

                lista.innerHTML = '';
                features.forEach(feat => {
                    const p = feat.properties;
                    const end = montar(p);
                    if (!end) return;
                    const { principal, sub } = display(p);
                    const item = document.createElement('div');
                    item.className = 'ph-item';
                    item.innerHTML = `${ICO_PIN}
                        <div>
                          <div class="ph-main">${principal}</div>
                          ${sub ? `<div class="ph-sub">${sub}</div>` : ''}
                        </div>`;
                    item.addEventListener('mousedown', e => {
                        e.preventDefault();
                        input.value = end;
                        fechar();
                        input.dispatchEvent(new Event('change', { bubbles: true }));
                        input.focus();
                    });
                    lista.appendChild(item);
                });
                idxSel = -1;

            } catch (e) {
                if (e.name !== 'AbortError')
                    lista.innerHTML = `<div class="ph-msg">Erro na busca.</div>`;
            }
        }

        input.addEventListener('input', function () {
            clearTimeout(timer);
            const v = this.value.trim();
            if (v.length < 4) { fechar(); return; }
            timer = setTimeout(() => buscar(v), 400);
        });

        input.addEventListener('keydown', function (e) {
            if (!lista) return;
            const list = els();
            if (e.key === 'ArrowDown') { e.preventDefault(); idxSel = Math.min(idxSel + 1, list.length - 1); realcar(idxSel); }
            else if (e.key === 'ArrowUp') { e.preventDefault(); idxSel = Math.max(idxSel - 1, -1); realcar(idxSel); }
            else if (e.key === 'Enter' && idxSel >= 0) { e.preventDefault(); list[idxSel].dispatchEvent(new Event('mousedown')); }
            else if (e.key === 'Escape') fechar();
        });

        input.addEventListener('blur', () => setTimeout(fechar, 160));
        document.addEventListener('click', e => { if (!wrap.contains(e.target)) fechar(); });
    };

})();