import React from 'react'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

class ContentEditorForm extends React.Component {
  static propTypes = {
    handleSubmit: React.PropTypes.func.isRequired,
    onSave: React.PropTypes.func.isRequired,
    type: React.PropTypes.object.isRequired,
    loading: React.PropTypes.bool
  }

  render() {
    const { handleSubmit, type, loading } = this.props

    return (
      <form className='content-editor__form' disabled={loading}>
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
          <button className={'button is-disabled' + (loading ? ' is-loading' : '')}
            type='button' onClick={handleSubmit}>Save</button>
          <button className={'button is-primary' + (loading ? ' is-loading' : '')}
            type='button' onClick={handleSubmit}>Save and publish</button>
        </div>
      </form>
    )
  }
}

export default reduxForm({
  form: 'ContentEditorForm'
})(ContentEditorForm)
