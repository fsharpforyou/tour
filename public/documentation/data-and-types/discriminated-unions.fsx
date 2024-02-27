type ContactInfo =
    | EmailAddress of string
    | PhoneNumber of string

let contact contactInfo =
    match contactInfo with
    | EmailAddress email -> sprintf "Sending an email to %s" email
    | PhoneNumber number -> sprintf "Sending a text message to %s" number

let contactInfo = PhoneNumber "000-000-0000"
printfn "%s" (contact contactInfo)
