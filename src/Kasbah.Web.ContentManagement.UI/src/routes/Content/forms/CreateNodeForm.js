import React from 'react'
import { Field, reduxForm } from 'redux-form'
import ModalForm from 'components/ModalForm'

const CreateNodeForm = ({ handleSubmit, types, onClose, loading }) => (
  <ModalForm onClose={onClose} onSubmit={handleSubmit} loading={loading} title='Create node'>
    <div className='control'>
      <label className='label' htmlFor='Type'>Type</label>
      <span className='select is-fullwidth'>
        <Field name='type' component='select' autoFocus>
          {types
            .map((ent, index) => <option key={index} value={ent.alias}>{ent.displayName}</option>)}
        </Field>
      </span>
    </div>
    <div className='control'>
      <label className='label' htmlFor='alias'>Alias</label>
      <Field name='alias' component='input' type='text' className='input' />
    </div>
    <div className='control'>
      <label className='label' htmlFor='displayName'>Display name</label>
      <Field name='displayName' component='input' type='text' className='input' />
    </div>
  </ModalForm>
)

CreateNodeForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  loading: React.PropTypes.bool,
  onClose: React.PropTypes.func.isRequired,
  types: React.PropTypes.array.isRequired
}

export default reduxForm({
  form: 'CreateNodeForm'
})(CreateNodeForm)
