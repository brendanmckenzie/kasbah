import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import ModalForm from 'components/ModalForm'

const UserForm = ({ handleSubmit, onClose, loading }) => (
  <ModalForm onClose={onClose} onSubmit={handleSubmit} loading={loading} title='User detail'>
    <div className='columns'>
      <div className='column'>
        <div className='field'>
          <label className='label' htmlFor='username'>Username</label>
          <div className='control'>
            <Field name='username' component='input' type='username' className='input' autoFocus />
          </div>
        </div>
      </div>
      <div className='column'>
        <div className='field'>
          <label className='label' htmlFor='password'>Password</label>
          <div className='control'>
            <Field name='password' component='input' type='password' className='input' />
          </div>
        </div>
      </div>
    </div>
    <div className='columns'>
      <div className='column is-5'>
        <div className='field'>
          <label className='label' htmlFor='name'>Name</label>
          <div className='control'>
            <Field name='name' component='input' type='text' className='input' />
          </div>
        </div>
      </div>
      <div className='column'>
        <div className='field'>
          <label className='label' htmlFor='email'>Email</label>
          <div className='control'>
            <Field name='email' component='input' type='email' className='input' />
          </div>
        </div>
      </div>
    </div>
  </ModalForm>
)

UserForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  onClose: PropTypes.func.isRequired
}

export default reduxForm({
  form: 'UserForm'
})(UserForm)
