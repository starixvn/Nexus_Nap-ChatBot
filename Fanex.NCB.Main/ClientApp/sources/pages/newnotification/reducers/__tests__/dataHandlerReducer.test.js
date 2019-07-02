import * as reducers from '../../reducers/dataHandlerReducer'
import * as actions from '../../actions/dataHandlerActions'

describe('dataHandlerReducer', () => {
    describe('settingData', () => { 
        it('should return the initial state', () => {
            //Arrange
            const expected = null
            
            //Act
            const actual = reducers.settingData(undefined, {})
    
            //Assert
            expect(actual).toEqual(expected)
        })
        it('should handle settingData action', () => {
            //Arrange
            const data = {
                notificationTypes: [{ id: 1, name: 'a' }, { id: 2, name: 'b' }],
                products: [{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }]
            }
            const action = actions.setSettings(data)
    
            //Act
            const actual = reducers.settingData(undefined, action)
    
            //Assert
            expect(actual).toEqual(data)
        }) 
    })

    describe('notificationTypes', () => { 
        it('should return the initial state', () => {
            //Arrange
            const expected = null

            //Act
            const actual = reducers.notificationTypes(undefined, {})
    
            //Assert
            expect(actual).toEqual(expected)
        })

        it('should handle notificationTypes action', () => {
             //Arrange
             const data = [{ id: 1, name: 'a' }, { id: 2, name: 'b' }]
             const action = actions.setNotificationTypes(data)

             //Act
             const actual = reducers.notificationTypes(undefined, action)
     
             //Assert
             expect(actual).toEqual(data)
        })
    })

    describe('notificationTemplates', () => { 
        it('should return the initial state', () => {
            //Arrange
            const expected = null

            //Act
            const actual = reducers.notificationTemplates(undefined, {})
    
            //Assert
            expect(actual).toEqual(expected)
        })

        it('should handle setTemplateTypes action', () => {
             //Arrange
             const data = [{ id: 1, name: 'template 1' }, { id: 2, name: 'template 2' }];
             const action = actions.setTemplateTypes(data)

             //Act
             const actual = reducers.notificationTemplates(undefined, action)
     
             //Assert
             expect(actual).toEqual(data)
        })
    })

    describe('template', () => { 
        it('should return the initial state', () => {
            //Arrange
            const expected = null

            //Act
            const actual = reducers.template(undefined, {})
    
            //Assert
            expect(actual).toEqual(expected)
        })

        it('should handle setTemplateTypes action', () => {
             //Arrange
             const data = {
                title: 'title hh {UM date}',
                content: 'good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc'
            }             
            const action = actions.setTemplate(data)

             //Act
             const actual = reducers.template(undefined, action)
     
             //Assert
             expect(actual).toEqual(data)
        })
    })
})