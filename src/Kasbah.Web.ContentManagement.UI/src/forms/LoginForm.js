import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'

const LoginForm = ({ handleSubmit, loading, errorMessage }) => (
  <form onSubmit={handleSubmit}>
    <div className='field'>
      <label className='label' htmlFor='username'>Username</label>
      <div className='control'>
        <Field name='username' component='input' type='text' className='input' autoFocus />
      </div>
    </div>
    <div className='field'>
      <label className='label' htmlFor='password'>Password</label>
      <div className='control'>
        <Field name='password' component='input' type='password' className='input' />
      </div>
    </div>
    {errorMessage && (
      <div className='notification is-danger'>
        {errorMessage}
      </div>
    )}
    <div className='level'>
      <div className='level-left' />
      <div className='level-center'>
        <button className={'button is-primary ' + (loading ? 'is-loading' : '')}>Login</button>
      </div>
      <div className='level-right' />
    </div>
  </form>
)

LoginForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  errorMessage: PropTypes.string
}

export default reduxForm({ form: 'LoginForm' })(LoginForm)
