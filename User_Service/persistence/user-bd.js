// DATA STORE

let users = {}
let now = new Date()
let yesterday = now.getDate() + 1

users.Davide = {
    id: 'Davide',
    name: 'Davide',
    email: 'davide@email.com',
    password:
        '52cbf0cef5652ce98529071b3c12532d85d692faa374fec78496fbfc57899406f5abd8ee9ad03e8f24b44ecb570ff5b675664e44ffc0985b2e49772867b0f2e4',
    salt: '150bbc4ec13e630cdec361c54e057fd39db7f81c3a6d76525136dfac105d60b9',
    roles: ['subscriber'],
    createdOn: now,
    updatedOn: now,
    _rev: '0'
}

users.Sara = {
    id: 'Sara',
    name: 'Sara',
    email: 'sara@email.com',
    password:
        '3a27aaa18a8dcca95c926b60b9aff19b9f5c1980403fa4d1394dc9a44339f6c25834d0be4fe59d2fff326227dd7921cbf944e92b39b8fb9b38fa34aa5a726c70',
    salt: '62b91b2bb090884fd5de669a79b1fd2232bc259cea7e93c66b8a31b8190805b9',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Simao = {
    id: 'Simao',
    name: 'Simão',
    email: 'simao@email.com',
    password:
        '79b911d29995cbb20e42619e983eac81b10455293d258e3f76d70b4efb1596a7c239ac3a5152d77227a445ef28986e9209408848de18423a168807514f3a3db5',
    salt: '666c22bf6d8b88ab84b04a72acce9426b7b42d36d86ce4f2c3d98925b93289f9',
    roles: ['subscriber'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Admin = {
    id: 'Admin',
    name: 'Zé do Pipo',
    email: 'ze@email.com',
    password:
        '3ede5202fd6baec2dac57643134982fac18587b8b5e95e7a788bdbdfec31e09d6cb159e6cb889c1f0033256a03a2dfd63abc63295eb283da88e6471640f3afe5',
    salt: '3164d39ee0074d2eb8d2cf47de63a140ae37607db3d85e16bd306a68322c5071',
    roles: ['admin'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Mario = {
    id: 'Mario',
    name: 'Zé do Pipo',
    email: 'marito@email.com',
    password:
        '8c196a9af01d6d4f2a2845a7c04d8591f89e18b3ed52c806508543130c5342fb5f89d578a9ff7b4b1db059a090dbd1b31005d10a086909f5db0c7a8650d690cc',
    salt: 'a3b4a35ae0d6c3114206122a857bdb62467ff7a448d3baf6cbf6c37103c0f228',
    roles: ['marketing-director'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Cecilia = {
    id: 'Cecilia',
    name: 'Cecilia',
    email: 'cecilia@email.com',
    password:
        'd6ffef8105bb1e03a705fb481ceb624419be2eb8124c2e6991bf419f55135bf366ab691cf06b3e0716c9c9db2e861422a7262fb190db5bad4f37feeef3f10782',
    salt: '18780e73387b646e1ee10b5e7123c44127bffb6ae707f4cb520624fc1b8ae9cd',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Narciso = {
    id: 'Narciso',
    name: 'Narciso',
    email: 'narciso@email.com',
    password:
        '7086c5ec97431ef4c409156a8d3a7284adc9b306754c94fe0102434127481f3d2069aa6eb9d1607e695a41ce61aab7fe63254491e65fdde55b6dfbc241797f9e',
    salt: '9a617ea4c63597e3e899e3b810461c249979130fd66ec8a2eef3d55e62df400f',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Arnaldo = {
    id: 'Arnaldo',
    name: 'Arnaldo',
    email: 'arnaldo@email.com',
    password:
        'b955728429a135badc589bc3cf00b19395bbde1e52f995fd2dbe94eb89579d0d2df91f945b6ed7ad3e11b572ad41c5f800a5181d50e9d359a4dff357882b5b1a',
    salt: '68972969265096c4a02d6caafe5a65043537cf340bd85d233826eb5bd5b29c53',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Sandrine = {
    id: 'Sandrine',
    name: 'Sandrine',
    email: 'sandrine@email.com',
    password:
        'bfa6784c044b229b7d6e05a2a14a14745bf90075e44fe1a05cf93259e8d5806199d8a49122c77774112b27f88a6eecc6195c9bb1384971ef664a39c508d9f169',
    salt: '6698fdde7697df0ad0ca900c42c2fdeb8f55a8e498d98a321a442b092f26910c',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

users.Dora = {
    id: 'Dora',
    name: 'Dora',
    email: 'dora@email.com',
    password:
        '3dbaf95d4d212a266f080afbcbc4f74ce840d80302529f5cdc69ae917da5873bbfd50c70a408e62871ea4ba1f1f54b5c50121fb5474608311afadfa2fe4c0737',
    salt: '9eef1fa5381942b529199242a4f0f8fd033564552eb20276024aca3e5afcaaa7',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}
    
users.Bernardino = {
    id: 'Bernardino',
    name: 'Bernardino',
    email: 'bernardino@email.com',
    password:
        '88fa66c7f778b5b9cdb18768b77fe84abe0a59a20541e5a37078c81090b28361422a977df7b2764504e3fdc99589b7543c22c12b663b6225494041eb0fc2f325',
    salt: 'ca1c4cf58852b55bacdbd7376737dfe418739a4390d1590f235eba58159c7ab2',
    roles: ['user'],
    createdOn: yesterday,
    updatedOn: now,
    _rev: '0'
}

//
// exports
//
exports.data = {
    users
}
