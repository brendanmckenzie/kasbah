import React from 'react'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

const ContentEditorForm = ({ handleSubmit, onSave, type, loading, publishing }) => (
  <form onSubmit={handleSubmit} className='content-editor__form' disabled={loading || publishing}>
    <Tabs>
      {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
        <Tab key={index} title={ent}>
          <div>
            {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
              <div key={fldIndex} className='control'>
                <label className='label' htmlFor={fld.alias}>{fld.displayName}</label>
                <Field
                  name={fld.alias}
                  id={fld.alias}
                  component={kasbah.getEditor(fld.editor)}
                  options={fld.options}
                  type={fld.type}
                  className='input' />
                {fld.helpText && <span className='help'>{fld.helpText}</span>}
              </div>
            ))}
          </div>
        </Tab>
      ))}
    </Tabs>
    <hr />
    <div className='has-text-right'>
      <button className={'button ' + (loading) ? 'is-loading' : ''} type='button' onClick={onSave}>Save</button>
      <button className={'button is-primary' + (loading) ? ' is-loading' : ''}>Save and publish</button>
    </div>
  </form >
)

ContentEditorForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  onSave: React.PropTypes.func.isRequired,
  type: React.PropTypes.object.isRequired,
  loading: React.PropTypes.bool,
  publishing: React.PropTypes.bool,
  error: React.PropTypes.string
}

export default reduxForm({
  form: 'ContentEditorForm'
})(ContentEditorForm)
