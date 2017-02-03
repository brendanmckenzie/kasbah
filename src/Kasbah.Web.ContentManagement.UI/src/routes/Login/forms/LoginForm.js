import React from 'react'
import { Field, reduxForm } from 'redux-form'

const LoginForm = ({ handleSubmit, loading, payload }) => (
  <form onSubmit={handleSubmit}>
    <div className='control'>
      <label className='label' htmlFor='username'>Username</label>
      <Field name='username' component='input' type='text' className='input' autoFocus />
    </div>
    <div className='control'>
      <label className='label' htmlFor='password'>Password</label>
      <Field name='password' component='input' type='password' className='input' />
    </div>
    {payload && payload.errorMessage && (
      <div className='notification is-danger'>
        {payload.errorMessage}
      </div>
    )}
    <div className='has-text-centered'>
      <button className={'button is-primary ' + (loading ? 'is-loading' : '')}>Login</button>
    </div>
  </form>
)

LoginForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  loading: React.PropTypes.bool,
  payload: React.PropTypes.object
}

export default reduxForm({
  form: 'LoginForm'
})(LoginForm)
