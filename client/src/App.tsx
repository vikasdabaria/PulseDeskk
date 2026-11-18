import React, { useState,useEffect } from "react";
interface Ticket{
id:number;
title:string;
description:string;
status:string;
priority:string;
createdByUserId: number;
createdByName: string;
assignedToUserId?: number;
assignedToName?: string;
createdAtUtc: string;
}



function App(){
  const [email,setEmail]=useState('');
 const  [password,setPassword]=useState('');
const [loading, setLoading] = useState(false);
const [error, setError] = useState('');
const [user,setUser]=useState<any>(null);

/* Tickets state start from here */
const[tickets,setTickets]=useState<Ticket[]>([]);
const [ticketsLoading,setTicketsLoading]=useState(false);
const [title,setTitle]=useState('');
const[description,setDescription]=useState('');
const[priority,setPriority]=useState('2');
const[creating,setCreating]=useState(false);


useEffect(()=>{

const checkUserSession=async()=>{

  const token=localStorage.getItem('token');
if(!token) return;

try{
const response=await fetch('/api/auth/me',{
  headers: {
            'Authorization': `Bearer ${token}`
          }
});

if(response.ok){
 const userdata=await response.json(); 
 setUser(userdata);
}else{
  localStorage.removeItem('token');
}
}
catch(err){
console.error('session restore failed',err);
}
};
checkUserSession();

},[]);

useEffect(()=>{
  if(user){
  fetchTicket();
}
},[user]);

const fetchTicket=async()=>{
const token=localStorage.getItem('token');
if(!token) return;

setTicketsLoading(true);

try{
const response=await fetch('/api/tickets',{
  headers:{
    'Authorization':`Bearer ${token}`
  }
});
if(response.ok){
  const data: Ticket[]=await response.json();
  setTickets(data); 
}else{
  console.error('failed to fetch tickets');
}

}catch(err){
console.error('Error fetching tickets:',err)
}finally{
  setTicketsLoading(false);
}
};


 const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try{
const response =await fetch('/api/auth/login',{
method:'POST',headers:{ 'Content-Type': 'application/json',},body:JSON.stringify({ email, password }),
});

const data=await response.json();

if(!response.ok){
  throw new Error(data.error || 'Login failed')
}

localStorage.setItem('token',data.token);
setUser(data);

    }
    catch(err: any){
setError(err.message);
    }  finally{
      setLoading(false);
    }
    
  };
   const handleLogout=()=>{
localStorage.removeItem('token');
setUser(null);
setTickets([]);
  };

  const handleCreateTicket=async(e:React.FormEvent)=>{
e.preventDefault();
if(!title.trim()|| !description.trim()){
  alert('please enter both title and description');
  return;
}
  const token=localStorage.getItem('token');
  if(!token) return;

  setCreating(true);

  try{
const response=await fetch('/api/tickets',{
  method:'POST',
  headers:{
    'Content-Type':'application/json',
    'Authorization':`Bearer ${token}`
  },
  body:JSON.stringify({
    title,description,priority:Number(priority)
  })
});
if(!response.ok) {
  const err=await response.json();
  throw new Error(err.message||'Failed to create ticket');
}

const newTicket:Ticket=await response.json();

setTickets(prevTickets=>[newTicket,...prevTickets]);
setTitle('');
setDescription('');
setPriority('2');

  }catch(err:any){
alert(err.message)
  }finally{
    setCreating(false);
  }
};
  



  if(user){
    return(
      <div style={{padding:'20px',fontFamily:'sans-serif'}}>
        <h1>🎉 Welcome to PulseDesk Dashboard</h1>

        <p><strong>Name:</strong>{user.fullName}</p>
        <p><strong>Email:</strong> {user.email}</p>
        <p><strong>Role:</strong> {user.role}</p>
        <p><strong>Tenant ID:</strong> {user.tenantId}</p>
        <button onClick={handleLogout} style={{ marginTop: '20px' }}>
          Logout
        </button>

        <hr style={{margin:'25px 0'}}></hr>

<h2>Create a support a ticket</h2>

<form onSubmit={handleCreateTicket} style={{maxWidth:'400px'}}>

<div style={{marginBottom:'10px'}}>
  <input type="text" placeholder="Title (e.g printer not working)" value={title}
  onChange={(e)=>setTitle(e.target.value)} style={{width:'100%',padding:'8px'}}>
  </input>
</div>

<div style={{marginBottom:'10px'}}>
  <textarea placeholder="Detailed description of the issue..." value={description}
  onChange={(e)=>setDescription(e.target.value)} rows={3} style={{width:'100%',padding:'8px'}}></textarea>
</div>

<div style={{marginBottom:'10px'}}>

  <label style={{display:'block',marginBottom:'5px'}}>
    <strong>Priority:</strong>
  </label>
  <select value={priority} onChange={(e)=>setPriority(e.target.value)} style={{width:'100%',padding:'8px'}}>
    <option value="1">Low</option>
    <option value="2">Medium</option>
    <option value="3">High</option>
    <option value="4">Urgent</option>
  </select>
</div>
<button type="submit" disabled={creating} style={{padding:'8px 16px'}}>
  {creating?'Creating Ticket...':'Create Ticket'}
</button>

</form>

<hr style={{margin:'25px 0'}}></hr>
<h2>your Support Ticket</h2>
{ticketsLoading ? (<p>Loading tickets...</p>):tickets.length===0?(<p>
  No ticket found. create your first ticket!</p>):(<table border={1}
  cellPadding={8} style={{borderCollapse:'collapse',width:'100%',maxWidth:'800px'
    ,marginTop:'10px'
  }}>
<thead>
  <tr style={{backgroundColor:'#f0f0f0',textAlign:'left'}}>
    <th>ID</th>
    <th>Title</th>
    <th>Priority</th>
    <th>Status</th>
    <th>Created By</th>
    <th>Created At</th>
  </tr>
</thead>
<tbody>
  {tickets.map((ticket)=>(<tr key={ticket.id}>
    <td>#{ticket.id}</td>
     <td><strong>{ticket.title}</strong></td>
                  <td>{ticket.priority}</td>
                  <td>{ticket.status}</td>
                  <td>{ticket.createdByName}</td>
                  <td>{new Date(ticket.createdAtUtc).toLocaleDateString()}</td>
  </tr>))}
</tbody>

  </table>)}

      </div>
    );}
 

  return(
    <div style={{padding:'20px',fontFamily:'sans-serif'}}>

      <h1>PulseDesk Portal</h1>

<form onSubmit={handleLogin}>
<div>
          <input
            type="email"
            placeholder="Enter Your Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
        <div style={{ marginTop: '10px' }}>
          <input
            type="password"
            placeholder="Enter Your Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
 <button type="submit" disabled={loading} style={{ marginTop: '15px' }}>
          {loading?'Logging in...': 'Sign In'}
        </button>
</form>
     

      
    </div>
  );
}

export default App;